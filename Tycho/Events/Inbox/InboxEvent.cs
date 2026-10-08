using System;
using Tycho.Events.Model;

namespace Tycho.Events.Inbox
{
    internal sealed class InboxEvent
    {
        public Guid EventId => Event.Id;

        public Guid ClaimId { get; }

        public Event Event { get; }

        public InboxEvent(Guid claimId, Event @event)
        {
            ClaimId = claimId;
            Event = @event;
        }
    }
}
