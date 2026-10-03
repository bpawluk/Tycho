using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tycho.Persistence.EFCore.Inbox;
using Tycho.Persistence.EFCore.Logging;
using Tycho.Persistence.EFCore.Outbox;

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
        IInboxCleaner inbox = services.GetRequiredService<IInboxCleaner>();

        if (options.Inbox.FullCleanupRetention is TimeSpan inboxRetention)
        {
            DateTime cutoff = GetCutoff(utcNow, inboxRetention);
            int affectedCount = await inbox.CleanEntriesAsync(cutoff, cancellationToken).ConfigureAwait(false);
            if (affectedCount > 0)
            {
                logger?.RetentionCleanupAffectedEntries("Deleting messages", affectedCount, "Inbox", cutoff);
            }
        }

        if (options.Inbox.PayloadRetention is TimeSpan inboxPayloadRetention)
        {
            DateTime cutoff = GetCutoff(utcNow, inboxPayloadRetention);
            int affectedCount = await inbox.CleanPayloadsAsync(cutoff, cancellationToken).ConfigureAwait(false);
            if (affectedCount > 0)
            {
                logger?.RetentionCleanupAffectedEntries("Clearing payloads", affectedCount, "Inbox", cutoff);
            }
        }

        IOutboxCleaner outbox = services.GetRequiredService<IOutboxCleaner>();

        if (options.Outbox.FullCleanupRetention is TimeSpan outboxRetention)
        {
            DateTime cutoff = GetCutoff(utcNow, outboxRetention);
            int affectedCount = await outbox.CleanEntriesAsync(cutoff, cancellationToken).ConfigureAwait(false);
            if (affectedCount > 0)
            {
                logger?.RetentionCleanupAffectedEntries("Deleting messages", affectedCount, "Outbox", cutoff);
            }
        }

        if (options.Outbox.PayloadRetention is TimeSpan outboxPayloadRetention)
        {
            DateTime cutoff = GetCutoff(utcNow, outboxPayloadRetention);
            int affectedCount = await outbox.CleanPayloadsAsync(cutoff, cancellationToken).ConfigureAwait(false);
            if (affectedCount > 0)
            {
                logger?.RetentionCleanupAffectedEntries("Clearing payloads", affectedCount, "Outbox", cutoff);
            }
        }
    }

    private static DateTime GetCutoff(DateTime utcNow, TimeSpan retention)
    {
        return retention.Ticks >= utcNow.Ticks ? DateTime.MinValue : utcNow - retention;
    }
}
