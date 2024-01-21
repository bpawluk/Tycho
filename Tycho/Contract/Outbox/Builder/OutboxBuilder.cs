using System;
using System.Collections.Generic;
using System.Linq;
using Tycho.DependencyInjection;
using Tycho.Messaging;
using Tycho.Messaging.Payload;

namespace Tycho.Contract.Outbox.Builder
{
    internal partial class OutboxBuilder : IOutboxDefinition, IOutboxConsumer
    {
        private readonly IInstanceCreator _instanceCreator;
        private readonly IMessageRouter _moduleInbox;
        private readonly IDictionary<Type, bool> _messageRegistry;

        public OutboxBuilder(IInstanceCreator instanceCreator, IMessageRouter moduleInbox)
        {
            _instanceCreator = instanceCreator;
            _moduleInbox = moduleInbox;
            _messageRegistry = new Dictionary<Type, bool>();
        }

        public IMessageBroker Build()
        {
            ValidateIfAllMethodsAreHandled();
            return new MessageBroker(_moduleInbox);
        }

        private void AddMessageDefinition(Type messageType, string messageKind)
        {
            if (!_messageRegistry.TryAdd(messageType, false))
            {
                throw new ArgumentException(
                    $"The {messageType.Name} {messageKind} is already defined for this module",
                    nameof(messageType));
            }
        }

        private void ValidateIfMessageIsDefined(Type messageType, string messageKind)
        {
            if (!_messageRegistry.ContainsKey(messageType))
            {
                throw new InvalidOperationException(
                    $"Could not register a message handler because " +
                    $"the {messageType.Name} {messageKind} is not defined for this module");
            }
        }

        private void ValidateIfAllMethodsAreHandled()
        {
            var missingHandlers = _messageRegistry.Where(entry => !entry.Value && ShouldMessageBeHandled(entry.Key));
            if (missingHandlers.Any())
            {
                throw new InvalidOperationException(
                    $"Could not construct a message broker for this module " +
                    $"because one or more messages are missing required handlers. " +
                    $"Please provide handler for: {string.Join(',', missingHandlers.Select(entry => entry.Key))}");
            }
        }

        private void MarkMessageAsHandled(Type type) => _messageRegistry[type] = true;

        private bool ShouldMessageBeHandled(Type type) => !typeof(IEvent).IsAssignableFrom(type);
    }
}
