using System;
using Tycho.Events;
using Tycho.Modules;
using Tycho.Utils;

namespace Tycho.Apps
{
    /// <summary>
    /// An interface for declaring Events expected by a Tycho Application.
    /// </summary>
    [ReferencedBySourceGenerator]
    public interface IAppEvents
    {
        /// <summary>
        /// Declares that the Application expects Events of type <typeparamref name="TEvent"/>.
        /// </summary>
        /// <typeparam name="TEvent">The type of the expected Event.</typeparam>
        /// <returns>An expectation builder for the Event.</returns>
        [ReferencedBySourceGenerator]
        IAppEventExpectation<TEvent> Expects<TEvent>()
            where TEvent : class, IEvent;
    }

    /// <summary>
    /// Configures how an expected Event is handled or routed within an Application.
    /// </summary>
    /// <typeparam name="TEvent">The type of the expected Event.</typeparam>
    [ReferencedBySourceGenerator]
    public interface IAppEventExpectation<TEvent>
        where TEvent : class, IEvent
    {
        /// <summary>
        /// Declares that the Application will handle the expected Event using a Handler
        /// <typeparamref name="THandler"/>.
        /// </summary>
        /// <typeparam name="THandler">The type of Event Handler.</typeparam>
        [ReferencedBySourceGenerator]
        IAppEvents HandlesWith<THandler>()
            where THandler : class, IEventHandler<TEvent>;

        /// <summary>
        /// Forwards the expected Event to a Module of type <typeparamref name="TModule"/>.
        /// </summary>
        /// <typeparam name="TModule">The type of the Module that receives the Event.</typeparam>
        IAppEventExpectation<TEvent> ForwardsTo<TModule>()
            where TModule : TychoModule;

        /// <summary>
        /// Maps the expected Event to <typeparamref name="TTargetEvent"/> for routed delivery.
        /// </summary>
        /// <typeparam name="TTargetEvent">The target Event type.</typeparam>
        /// <param name="map">The Event mapper.</param>
        IAppMappedEventExpectation<TEvent, TTargetEvent> MapsTo<TTargetEvent>(Func<TEvent, TTargetEvent> map)
            where TTargetEvent : class, IEvent;
    }

    /// <summary>
    /// Configures routed delivery for a mapped Application Event.
    /// </summary>
    /// <typeparam name="TEvent">The type of the expected Event.</typeparam>
    /// <typeparam name="TTargetEvent">The target Event type.</typeparam>
    public interface IAppMappedEventExpectation<TEvent, TTargetEvent>
        where TEvent : class, IEvent
        where TTargetEvent : class, IEvent
    {
        /// <summary>
        /// Forwards the mapped Event to a Module of type <typeparamref name="TModule"/>.
        /// </summary>
        /// <typeparam name="TModule">The type of the Module that receives the mapped Event.</typeparam>
        IAppMappedEventExpectation<TEvent, TTargetEvent> ForwardsTo<TModule>()
            where TModule : TychoModule;
    }
}
