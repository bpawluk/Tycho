using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tycho.Persistence.EFCore.Inbox;
using Tycho.Persistence.EFCore.Outbox;

namespace Tycho.Persistence.EFCore.Retention;

internal sealed class PersistenceRetentionService(
    IServiceScopeFactory scopeFactory,
    PersistenceRetentionOptions options,
    TimeProvider? timeProvider = null) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        TimeProvider clock = timeProvider ?? TimeProvider.System;

        try
        {
            await Task.Delay(TimeSpan.FromSeconds(Random.Shared.Next(300, 601)), clock, stoppingToken).ConfigureAwait(false);

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
                catch
                {
                    // TODO: Log the exception and continue with the next cleanup cycle.
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
            await inbox.CleanEntriesAsync(GetCutoff(utcNow, inboxRetention), cancellationToken).ConfigureAwait(false);
        }

        if (options.Inbox.PayloadRetention is TimeSpan inboxPayloadRetention)
        {
            await inbox.CleanPayloadsAsync(GetCutoff(utcNow, inboxPayloadRetention), cancellationToken).ConfigureAwait(false);
        }

        IOutboxCleaner outbox = services.GetRequiredService<IOutboxCleaner>();

        if (options.Outbox.FullCleanupRetention is TimeSpan outboxRetention)
        {
            await outbox.CleanEntriesAsync(GetCutoff(utcNow, outboxRetention), cancellationToken).ConfigureAwait(false);
        }

        if (options.Outbox.PayloadRetention is TimeSpan outboxPayloadRetention)
        {
            await outbox.CleanPayloadsAsync(GetCutoff(utcNow, outboxPayloadRetention), cancellationToken).ConfigureAwait(false);
        }
    }

    private static DateTime GetCutoff(DateTime utcNow, TimeSpan retention)
    {
        return retention.Ticks >= utcNow.Ticks ? DateTime.MinValue : utcNow - retention;
    }
}
