using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tycho.Identity.Events;
using Tycho.Logging;
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
            cancellationToken.ThrowIfCancellationRequested();

            InboxEvent? inboxEvent = _event;
            if (inboxEvent is null)
            {
                AsyncServiceScope scope = _internals.CreateAsyncScope();
                await using ConfiguredAsyncDisposable scopeDisposal = scope.ConfigureAwait(false);

                ILogger<InboxProcessorJob>? logger = scope.ServiceProvider.GetService<ILogger<InboxProcessorJob>>();
                logger?.InboxJobIsMissing();

                return;
            }

            if (!await TryProcessEventAsync(inboxEvent, cancellationToken).ConfigureAwait(false))
            {
                AsyncServiceScope scope = _internals.CreateAsyncScope();
                await using ConfiguredAsyncDisposable scopeDisposal = scope.ConfigureAwait(false);

                ILogger<InboxProcessorJob>? logger = scope.ServiceProvider.GetService<ILogger<InboxProcessorJob>>();
                IInboxConsumer inbox = scope.ServiceProvider.GetRequiredService<IInboxConsumer>();

                bool markedAsFailed = await inbox.MarkAsFailedAsync(inboxEvent.EventId, inboxEvent.ClaimId, CancellationToken.None).ConfigureAwait(false);
                if (!markedAsFailed)
                {
                    logger?.InboxMessageStatusUpdateFailed(inboxEvent.EventId, inboxEvent.ClaimId);
                }
            }
        }

        private async Task<bool> TryProcessEventAsync(InboxEvent inboxEvent, CancellationToken cancellationToken)
        {
            AsyncServiceScope scope = _internals.CreateAsyncScope();
            await using ConfiguredAsyncDisposable scopeDisposal = scope.ConfigureAwait(false);

            ILogger<InboxProcessorJob>? logger = scope.ServiceProvider.GetService<ILogger<InboxProcessorJob>>();
            IInboxConsumer inbox;

            try
            {
                inbox = scope.ServiceProvider.GetRequiredService<IInboxConsumer>();
                var handlerProvider = new EventHandlerProvider(scope.ServiceProvider);

                IEventHandler eventHandler = inboxEvent.Event.GetHandlerFrom(handlerProvider);
                if (eventHandler is ITransactionalEventHandler)
                {
                    async Task HandleAndAcknowledgeAsync(CancellationToken token)
                    {
                        await HandleEventAsync(inboxEvent, eventHandler, token).ConfigureAwait(false);

                        bool markedAsHandled = await inbox.MarkAsHandledAsync(inboxEvent.EventId, inboxEvent.ClaimId, token).ConfigureAwait(false);
                        if (!markedAsHandled)
                        {
                            throw new InvalidOperationException($"Failed to mark inbox entry with ID {inboxEvent.EventId} as handled for claim {inboxEvent.ClaimId}.");
                        }
                    }

                    ITransaction transaction = scope.ServiceProvider.GetRequiredService<ITransaction>();
                    await transaction.ExecuteAsync(HandleAndAcknowledgeAsync, cancellationToken).ConfigureAwait(false);

                    return true;
                }

                await HandleEventAsync(inboxEvent, eventHandler, cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception)
            {
                logger?.InboxMessageProcessingFailed(inboxEvent.EventId, exception);
                return false;
            }

            bool markedAsHandled = await inbox.MarkAsHandledAsync(inboxEvent.EventId, inboxEvent.ClaimId, CancellationToken.None).ConfigureAwait(false);
            if (!markedAsHandled)
            {
                logger?.InboxMessageStatusUpdateFailed(inboxEvent.EventId, inboxEvent.ClaimId);
            }

            return true;
        }

        private static Task HandleEventAsync(InboxEvent inboxEvent, IEventHandler eventHandler, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return inboxEvent.Event.HandleWith(eventHandler, cancellationToken);
        }
    }
}
