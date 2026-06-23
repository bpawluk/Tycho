using System;
using Tycho.Events;
using Tycho.Utils;

namespace Tycho.Modules
{
    /// <summary>
    /// An interface for declaring the Events expected by a Module.
    /// </summary>
    [ReferencedBySourceGenerator]
    public interface IModuleEvents
    {
        /// <summary>
        /// Declares that the Module expects Events of type <typeparamref name="TEvent"/>.
        /// </summary>
        /// <typeparam name="TEvent">The type of the expected Event.</typeparam>
        /// <returns>An expectation builder for the Event.</returns>
        [ReferencedBySourceGenerator]
        IModuleEventExpectation<TEvent> Expects<TEvent>()
            where TEvent : class, IEvent;
    }

    /// <summary>
    /// Configures how an expected Module Event is handled, forwarded, or exposed.
    /// </summary>
    /// <typeparam name="TEvent">The type of the expected Event.</typeparam>
    [ReferencedBySourceGenerator]
    public interface IModuleEventExpectation<TEvent>
        where TEvent : class, IEvent
    {
        /// <summary>
        /// Declares that the Module will handle the expected Event using Handler
        /// <typeparamref name="THandler"/>.
        /// </summary>
        /// <typeparam name="THandler">The type of Event Handler.</typeparam>
        [ReferencedBySourceGenerator]
        IModuleEvents HandlesWith<THandler>()
            where THandler : class, IEventHandler<TEvent>;

        /// <summary>
        /// Forwards the expected Event to Module <typeparamref name="TModule"/>.
        /// </summary>
        /// <typeparam name="TModule">The type of the Module that receives the Event.</typeparam>
        IModuleEventExpectation<TEvent> ForwardsTo<TModule>()
            where TModule : TychoModule;

        /// <summary>
        /// Exposes the expected Event to the Module's parent.
        /// </summary>
        IModuleEventExpectation<TEvent> Exposes();

        /// <summary>
        /// Maps the expected Event to <typeparamref name="TTargetEvent"/> for routed delivery.
        /// </summary>
        /// <typeparam name="TTargetEvent">The target Event type.</typeparam>
        /// <param name="map">The Event mapper.</param>
        IModuleMappedEventExpectation<TEvent, TTargetEvent> MapsTo<TTargetEvent>(Func<TEvent, TTargetEvent> map)
            where TTargetEvent : class, IEvent;
    }

    /// <summary>
    /// Configures routed delivery for a mapped Module Event.
    /// </summary>
    /// <typeparam name="TEvent">The type of the expected Event.</typeparam>
    /// <typeparam name="TTargetEvent">The target Event type.</typeparam>
    public interface IModuleMappedEventExpectation<TEvent, TTargetEvent>
        where TEvent : class, IEvent
        where TTargetEvent : class, IEvent
    {
        /// <summary>
        /// Forwards the mapped Event to Module <typeparamref name="TModule"/>.
        /// </summary>
        /// <typeparam name="TModule">The type of the Module that receives the mapped Event.</typeparam>
        IModuleMappedEventExpectation<TEvent, TTargetEvent> ForwardsTo<TModule>()
            where TModule : TychoModule;

        /// <summary>
        /// Exposes the mapped Event to the Module's parent.
        /// </summary>
        IModuleMappedEventExpectation<TEvent, TTargetEvent> Exposes();
    }
}
