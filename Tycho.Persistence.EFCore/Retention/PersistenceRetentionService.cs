using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tycho.Persistence.EFCore.Identity;
using Tycho.Persistence.EFCore.Logging;
using Tycho.Persistence.EFCore.Operations;

namespace Tycho.Persistence.EFCore.Retention;

internal sealed class PersistenceRetentionService(
    IServiceScopeFactory scopeFactory,
    PersistenceRetentionSettings options,
    TimeProvider? timeProvider = null,
    ILogger<PersistenceRetentionService>? logger = null) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        TimeProvider clock = timeProvider ?? TimeProvider.System;

        try
        {
            if (options.InitialDelay > TimeSpan.Zero)
            {
                await Task.Delay(options.InitialDelay, clock, stoppingToken).ConfigureAwait(false);
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await using AsyncServiceScope scope = scopeFactory.CreateAsyncScope();
                    await CleanupAsync(scope.ServiceProvider, clock.GetUtcNow().UtcDateTime, stoppingToken).ConfigureAwait(false);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    logger?.RetentionCleanupFailed(options.CleanupInterval, exception);
                }

                await Task.Delay(options.CleanupInterval, clock, stoppingToken).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            // Normal host shutdown.
        }
    }

    private async Task CleanupAsync(IServiceProvider services, DateTime utcNow, CancellationToken cancellationToken)
    {
        TychoDbContext dbContext = services.GetRequiredService<TychoDbContext>();
        PersistenceOwner owner = services.GetRequiredService<PersistenceOwner>();

        await RunCleanupAsync(
            options.Inbox.FullCleanupRetention,
            utcNow,
            (cutoff, token) => DeleteInboxEntriesCommand.ExecuteAsync(dbContext, owner, cutoff, token),
            "Deleting messages",
            "Inbox",
            cancellationToken).ConfigureAwait(false);

        await RunCleanupAsync(
            options.Inbox.PayloadRetention,
            utcNow,
            (cutoff, token) => ClearInboxPayloadsCommand.ExecuteAsync(dbContext, owner, cutoff, token),
            "Clearing payloads",
            "Inbox",
            cancellationToken).ConfigureAwait(false);

        await RunCleanupAsync(
            options.Outbox.FullCleanupRetention,
            utcNow,
            (cutoff, token) => DeleteOutboxEntriesCommand.ExecuteAsync(dbContext, owner, cutoff, token),
            "Deleting messages",
            "Outbox",
            cancellationToken).ConfigureAwait(false);

        await RunCleanupAsync(
            options.Outbox.PayloadRetention,
            utcNow,
            (cutoff, token) => ClearOutboxPayloadsCommand.ExecuteAsync(dbContext, owner, cutoff, token),
            "Clearing payloads",
            "Outbox",
            cancellationToken).ConfigureAwait(false);
    }

    private async Task RunCleanupAsync(
        TimeSpan? retention,
        DateTime utcNow,
        Func<DateTime, CancellationToken, Task<int>> clean,
        string operation,
        string store,
        CancellationToken cancellationToken)
    {
        if (retention is not TimeSpan duration)
        {
            return;
        }

        DateTime cutoff = GetCutoff(utcNow, duration);
        int affectedCount = await clean(cutoff, cancellationToken).ConfigureAwait(false);

        if (affectedCount > 0)
        {
            logger?.RetentionCleanupAffectedEntries(operation, affectedCount, store, cutoff);
        }
    }

    private static DateTime GetCutoff(DateTime utcNow, TimeSpan retention)
    {
        return retention.Ticks >= utcNow.Ticks ? DateTime.MinValue : utcNow - retention;
    }
}
