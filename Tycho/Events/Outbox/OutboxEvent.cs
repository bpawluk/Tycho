using System;
using Tycho.Events.Model;

namespace Tycho.Events.Outbox
{
    internal sealed class OutboxEvent
    {
        public Guid EventId => RoutedEvent.Id;

        public Guid ClaimId { get; }

        public SerializedRoutedEvent RoutedEvent { get; }

        public OutboxEvent(Guid claimId, SerializedRoutedEvent routedEvent)
        {
            ClaimId = claimId;
            RoutedEvent = routedEvent;
        }
    }
}
