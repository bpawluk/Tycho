using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tycho.Events.Model;
using Tycho.Identity.Events;
using Tycho.Processor;
using Tycho.Structure;
using Tycho.Transactions;
using Tycho.Utils;

namespace Tycho.Events.Inbox
{
    internal class InboxProcessorJob : IJob
    {
        private readonly Internals _internals;
        private RoutedEvent? _event;

        public InboxProcessorJob(Internals internals)
        {
            _internals = internals;
        }

        public InboxProcessorJob ForEvent(RoutedEvent routedEvent)
        {
            _event = routedEvent;
            return this;
        }

        [EntryPoint]
        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await using var scope = _internals.CreateAsyncScope();
            var logger = scope.ServiceProvider.GetService<ILogger<InboxProcessorJob>>();

            if (_event is null)
            {
                logger?.LogWarning("No event assigned for processing. Skipping execution.");
                return;
            }

            var inbox = scope.ServiceProvider.GetRequiredService<IInboxConsumer>();

            try
            {
                var handlerProvider = new EventHandlerProvider(scope.ServiceProvider);
                var eventHandler = _event!.GetHandlerFrom(handlerProvider);

                ITransaction? transaction = null;
                if (eventHandler is ITransactionalEventHandler)
                {
                    transaction = scope.ServiceProvider.GetRequiredService<ITransaction>();
                    await transaction.BeginAsync(cancellationToken).ConfigureAwait(false);
                }
                var isTransactionInProgress = transaction != null;

                try
                {
                    await _event!.HandleWith(eventHandler, cancellationToken).ConfigureAwait(false);
                    await inbox.MarkAsHandled(_event.Id, cancellationToken).ConfigureAwait(false);

                    if (isTransactionInProgress)
                    {
                        await transaction!.CommitAsync(cancellationToken).ConfigureAwait(false);
                    }
                }
                catch
                {
                    if (isTransactionInProgress)
                    {
                        await transaction!.RollbackAsync(cancellationToken).ConfigureAwait(false);
                    }
                    throw;
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Failed to process inbox entry with ID {entryId}", _event.Id);
                await inbox.MarkAsFailed(_event.Id, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
