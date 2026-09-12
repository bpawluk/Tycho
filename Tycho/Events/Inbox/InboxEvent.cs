using System;
using Tycho.Events.Model;

namespace Tycho.Events.Inbox
{
    internal sealed class InboxEvent
    {
        public Guid EventId => RoutedEvent.Id;

        public Guid ClaimId { get; }

        public RoutedEvent RoutedEvent { get; }

        public InboxEvent(Guid claimId, RoutedEvent routedEvent)
        {
            ClaimId = claimId;
            RoutedEvent = routedEvent;
        }
    }
}
