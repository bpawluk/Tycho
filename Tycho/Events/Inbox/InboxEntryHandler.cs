using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Tycho.Events.Handling;
using Tycho.Events.Serialization;

namespace Tycho.Events.Inbox
{
    internal class InboxEntryHandler : IInboxEntryHandler
    {
        private readonly IPayloadSerializer _payloadSerializer;
        private readonly IEventHandlerProvider _handlerProvider;
        private readonly ILogger<InboxEntryHandler> _logger;

        public InboxEntryHandler(
            IInboxConsumer outboxConsumer,
            IPayloadSerializer payloadSerializer,
            IEventHandlerProvider handlerProvider,
            InboxProcessorSettings? settings = null,
            ILogger<InboxEntryHandler>? logger = null)
        {
            _payloadSerializer = payloadSerializer;
            _handlerProvider = handlerProvider;
            _logger = logger ?? NullLogger<InboxEntryHandler>.Instance;
        }

        public async Task<bool> TryHandlingEntryAsync(InboxEntry entry, CancellationToken cancellationToken)
        {
            try
            {
                var eventHandler = _handlerProvider.GetHandler(entry.HandlerId);
                var eventData = _payloadSerializer.Deserialize(eventHandler.EventType, entry.Payload);
                await HandleWithReflection(eventData, eventHandler).ConfigureAwait(false);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to handle inbox entry with ID {entryId}", entry.Id);
                return false;
            }
        }

        private Task HandleWithReflection(IEvent eventData, IEventHandler eventHandler)
        {
            var handleMethod = eventHandler.GetType().GetMethod(nameof(IEventHandler<IEvent>.Handle));

            var handleResult = handleMethod.Invoke(
                eventHandler,
                new object[] {
                    eventData,
                    CancellationToken.None })
                as Task;

            if (handleResult is null)
            {
                throw new InvalidOperationException($"Failure invoking {eventHandler.GetType().Name} Handle method");
            }

            return handleResult;
        }
    }
}
