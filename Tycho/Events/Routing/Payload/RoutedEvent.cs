using System;
using Tycho.Events.Routing.Routes;
using Tycho.Registry;

namespace Tycho.Events.Routing.Payload
{
    internal class RoutedEvent : IRoutedEvent
    {
        public Guid Id { get; }

        public object Payload { get; }

        public Route Route { get; }

        public RoutedEvent(Guid id, object payload, Route route)
        {
            Id = id;
            Payload = payload;
            Route = route;
        }
    }

    internal class RoutedEvent<TEvent> : IRoutedEvent<TEvent> where TEvent : class, IEvent
    {
        public Guid Id { get; }

        public TEvent Payload { get; }

        public Route Route { get; }

        public RoutedEvent(Guid id, TEvent payload, EventHandlerIdentity handlerId)
        {
            Id = id;
            Payload = payload;
            Route = Route.WithHandler(handlerId);
        }
    }
}
