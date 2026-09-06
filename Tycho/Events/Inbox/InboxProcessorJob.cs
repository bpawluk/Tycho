using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
        private InboxEvent? _event;

        public InboxProcessorJob(Internals internals)
        {
            _internals = internals;
        }

        public InboxProcessorJob ForEvent(InboxEvent inboxEvent)
        {
            if (inboxEvent is null)
            {
                throw new ArgumentNullException(nameof(inboxEvent));
            }

            if (Interlocked.CompareExchange(ref _event, inboxEvent, null) != null)
            {
                throw new InvalidOperationException("An inbox event has already been assigned to this job.");
            }

            return this;
        }

        [EntryPoint]
        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            bool isInputValid = ValidateInput();
            if (!isInputValid)
            {
                return;
            }

            bool processingSucceeded = await ProcessEventAsync(cancellationToken).ConfigureAwait(false);
            if (!processingSucceeded)
            {
                await FailProcessingAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        private bool ValidateInput()
        {
            if (_event is null)
            {
                using IServiceScope scope = _internals.CreateScope();
                ILogger<InboxProcessorJob>? logger = scope.ServiceProvider.GetService<ILogger<InboxProcessorJob>>();
                logger?.LogWarning("No event assigned for processing. Skipping execution.");
                return false;
            }
            return true;
        }

        private async Task<bool> ProcessEventAsync(CancellationToken cancellationToken)
        {
            await using AsyncServiceScope scope = _internals.CreateAsyncScope();
            ILogger<InboxProcessorJob>? logger = scope.ServiceProvider.GetService<ILogger<InboxProcessorJob>>();

            try
            {
                IInboxConsumer inbox = scope.ServiceProvider.GetRequiredService<IInboxConsumer>();
                var handlerProvider = new EventHandlerProvider(scope.ServiceProvider);

                IEventHandler eventHandler = _event!.RoutedEvent.GetHandlerFrom(handlerProvider);
                if (eventHandler is ITransactionalEventHandler)
                {
                    ITransaction transaction = scope.ServiceProvider.GetRequiredService<ITransaction>();
                    await transaction.ExecuteAsync(token => HandleEventAsync(eventHandler, inbox, token), cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    await HandleEventAsync(eventHandler, inbox, cancellationToken).ConfigureAwait(false);
                }

                return true;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception)
            {
                logger?.LogError(exception, "Failed to process inbox entry with ID {entryId}", _event.EventId);
                return false;
            }
        }

        private async Task HandleEventAsync(IEventHandler eventHandler, IInboxConsumer inbox, CancellationToken cancellationToken)
        {
            await _event!.RoutedEvent.HandleWith(eventHandler, cancellationToken).ConfigureAwait(false);

            bool markedAsHandled = await inbox.MarkAsHandledAsync(_event.ClaimId, cancellationToken).ConfigureAwait(false);
            if (!markedAsHandled)
            {
                throw new InvalidOperationException($"Failed to mark inbox entry with ID {_event.EventId} as handled for claim {_event.ClaimId}.");
            }
        }

        private async Task FailProcessingAsync(CancellationToken cancellationToken)
        {
            await using AsyncServiceScope scope = _internals.CreateAsyncScope();
            ILogger<InboxProcessorJob>? logger = scope.ServiceProvider.GetService<ILogger<InboxProcessorJob>>();
            IInboxConsumer inbox = scope.ServiceProvider.GetRequiredService<IInboxConsumer>();

            bool markedAsFailed = await inbox.MarkAsFailedAsync(_event!.ClaimId, cancellationToken).ConfigureAwait(false);
            if (!markedAsFailed)
            {
                logger?.LogWarning("Failed to mark inbox entry with ID {entryId} as failed for claim {claimId}", _event.EventId, _event.ClaimId);
            }
        }
    }
}
