using System;
using Tycho.Events.Routing;

namespace Tycho.Persistence
{
    internal class OutboxEntry
    {
        public Guid Id { get; }

        public HandlerIdentity HandlerIdentity { get; }

        public object Payload { get; }

        public OutboxEntry(HandlerIdentity handlerIdentity, object payload)
        {
            Id = Guid.NewGuid();
            HandlerIdentity = handlerIdentity;
            Payload = payload;
        }

        public OutboxEntry(Guid id, HandlerIdentity handlerIdentity, object payload)
        {
            Id = id;
            HandlerIdentity = handlerIdentity;
            Payload = payload;
        }
    }
}