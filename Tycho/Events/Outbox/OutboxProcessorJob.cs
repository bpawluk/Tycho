using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tycho.Events.Delivery;
using Tycho.Logging;
using Tycho.Processor;
using Tycho.Structure;
using Tycho.Utils;

namespace Tycho.Events.Outbox
{
    internal class OutboxProcessorJob : IJob
    {
        private readonly Internals _internals;
        private OutboxEvent? _event;

        public OutboxProcessorJob(Internals internals)
        {
            _internals = internals;
        }

        public OutboxProcessorJob ForEvent(OutboxEvent outboxEvent)
        {
            if (outboxEvent is null)
            {
                throw new ArgumentNullException(nameof(outboxEvent));
            }

            if (Interlocked.CompareExchange(ref _event, outboxEvent, null) != null)
            {
                throw new InvalidOperationException("An outbox event has already been assigned to this job.");
            }

            return this;
        }

        [EntryPoint]
        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            AsyncServiceScope scope = _internals.CreateAsyncScope();
            await using ConfiguredAsyncDisposable scopeDisposal = scope.ConfigureAwait(false);

            ILogger<OutboxProcessorJob>? logger = scope.ServiceProvider.GetService<ILogger<OutboxProcessorJob>>();

            OutboxEvent? outboxEvent = _event;
            if (outboxEvent is null)
            {
                logger?.OutboxJobIsMissing();
                return;
            }

            IOutboxConsumer outbox = scope.ServiceProvider.GetRequiredService<IOutboxConsumer>();

            bool delivered = await TryDeliverAsync(outboxEvent, scope.ServiceProvider, logger, cancellationToken).ConfigureAwait(false);
            bool statusUpdated = delivered
                ? await outbox.MarkAsDeliveredAsync(outboxEvent.EventId, outboxEvent.ClaimId, CancellationToken.None).ConfigureAwait(false)
                : await outbox.MarkAsFailedAsync(outboxEvent.EventId, outboxEvent.ClaimId, CancellationToken.None).ConfigureAwait(false);

            if (!statusUpdated)
            {
                logger?.OutboxMessageStatusUpdateFailed(outboxEvent.EventId, outboxEvent.ClaimId);
            }
        }

        private static async Task<bool> TryDeliverAsync(
            OutboxEvent outboxEvent,
            IServiceProvider services,
            ILogger<OutboxProcessorJob>? logger,
            CancellationToken cancellationToken)
        {
            try
            {
                IEventDeliverer deliverer = services.GetRequiredService<IEventDeliverer>();
                cancellationToken.ThrowIfCancellationRequested();

                await deliverer.DeliverAsync(outboxEvent.RoutedEvent, cancellationToken).ConfigureAwait(false);
                return true;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception)
            {
                logger?.OutboxMessageDeliveryFailed(outboxEvent.EventId, exception);
                return false;
            }
        }
    }
}
