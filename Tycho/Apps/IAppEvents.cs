using System;
using Tycho.Events;
using Tycho.Modules;
using Tycho.Utils;

namespace Tycho.Apps
{
    /// <summary>
    /// An interface for declaring the events expected by a Tycho application.
    /// </summary>
    [ReferencedBySourceGenerator]
    public interface IAppEvents
    {
        /// <summary>
        /// Declares that the application expects events of type <typeparamref name="TEvent"/>.
        /// </summary>
        /// <typeparam name="TEvent">The type of the expected event.</typeparam>
        /// <returns>An expectation builder for the event.</returns>
        [ReferencedBySourceGenerator]
        IAppEventExpectation<TEvent> Expects<TEvent>()
            where TEvent : class, IEvent;
    }

    /// <summary>
    /// Configures how an expected event is handled or routed within an application.
    /// </summary>
    /// <typeparam name="TEvent">The type of the expected event.</typeparam>
    [ReferencedBySourceGenerator]
    public interface IAppEventExpectation<TEvent>
        where TEvent : class, IEvent
    {
        /// <summary>
        /// Declares that the application will handle the expected event using the handler <typeparamref name="THandler"/>.
        /// </summary>
        /// <typeparam name="THandler">The type of event handler.</typeparam>
        [ReferencedBySourceGenerator]
        IAppEvents HandlesWith<THandler>()
            where THandler : class, IEventHandler<TEvent>;

        /// <summary>
        /// Forwards the expected event to a module of type <typeparamref name="TModule"/>.
        /// </summary>
        /// <typeparam name="TModule">The type of the module that receives the event.</typeparam>
        IAppEventExpectation<TEvent> ForwardsTo<TModule>()
            where TModule : TychoModule;

        /// <summary>
        /// Maps the expected event to <typeparamref name="TTargetEvent"/> for routed delivery.
        /// </summary>
        /// <typeparam name="TTargetEvent">The target event type.</typeparam>
        /// <param name="map">The event mapper.</param>
        IAppMappedEventExpectation<TEvent, TTargetEvent> MapsTo<TTargetEvent>(Func<TEvent, TTargetEvent> map)
            where TTargetEvent : class, IEvent;
    }

    /// <summary>
    /// Configures routed delivery for a mapped application event.
    /// </summary>
    /// <typeparam name="TEvent">The type of the expected event.</typeparam>
    /// <typeparam name="TTargetEvent">The target event type.</typeparam>
    public interface IAppMappedEventExpectation<TEvent, TTargetEvent>
        where TEvent : class, IEvent
        where TTargetEvent : class, IEvent
    {
        /// <summary>
        /// Forwards the mapped event to a module of type <typeparamref name="TModule"/>.
        /// </summary>
        /// <typeparam name="TModule">The type of the module that receives the mapped event.</typeparam>
        IAppMappedEventExpectation<TEvent, TTargetEvent> ForwardsTo<TModule>()
            where TModule : TychoModule;
    }
}
