using System;
using Tycho.Events.Routing.Routes;

namespace Tycho.Events.Routing.Payload
{
    internal interface IRoutedBase
    {
        Guid Id { get; }

        Route Route { get; }
    }

    internal interface IRoutedEvent : IRoutedBase
    {
        object Payload { get; }
    }

    internal interface IRoutedEvent<out TEvent> : IRoutedBase
        where TEvent : class, IEvent
    {
        TEvent Payload { get; }
    }
}
