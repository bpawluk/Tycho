using System;
using Tycho.Identities;

namespace Tycho.Events.Inbox
{
    internal class InboxEntry
    {
        public Guid Id { get; }

        public object Payload { get; }

        public EventHandlerIdentity HandlerId { get; }

        public InboxEntry(Guid id, object payload, EventHandlerIdentity handlerId)
        {
            Id = id;
            Payload = payload;
            HandlerId = handlerId;
        }
    }
}
