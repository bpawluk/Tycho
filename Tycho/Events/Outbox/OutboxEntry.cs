using System;
using Tycho.Events.Routing.Routes;

namespace Tycho.Events.Outbox
{
    internal class OutboxEntry
    {
        public Guid Id { get; }

        public object Payload { get; }

        public Route Route { get; }

        public OutboxEntry(Guid id, object payload, Route route)
        {
            Id = id;
            Payload = payload;
            Route = route;
        }
    }
}
