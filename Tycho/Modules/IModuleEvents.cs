using System;
using Tycho.Events;
using Tycho.Utils;

namespace Tycho.Modules
{
    /// <summary>
    /// An interface for declaring the events expected by a module.
    /// </summary>
    [ReferencedBySourceGenerator]
    public interface IModuleEvents
    {
        /// <summary>
        /// Declares that the module expects events of type <typeparamref name="TEvent"/>.
        /// </summary>
        /// <typeparam name="TEvent">The type of the expected event.</typeparam>
        /// <returns>An expectation builder for the event.</returns>
        [ReferencedBySourceGenerator]
        IModuleEventExpectation<TEvent> Expects<TEvent>()
            where TEvent : class, IEvent;
    }

    /// <summary>
    /// Configures how an expected module event is handled, forwarded, or exposed.
    /// </summary>
    /// <typeparam name="TEvent">The type of the expected event.</typeparam>
    [ReferencedBySourceGenerator]
    public interface IModuleEventExpectation<TEvent>
        where TEvent : class, IEvent
    {
        /// <summary>
        /// Declares that the module will handle the expected event using the handler <typeparamref name="THandler"/>.
        /// </summary>
        /// <typeparam name="THandler">The type of event handler.</typeparam>
        [ReferencedBySourceGenerator]
        IModuleEvents HandlesWith<THandler>()
            where THandler : class, IEventHandler<TEvent>;

        /// <summary>
        /// Forwards the expected event to the module <typeparamref name="TModule"/>.
        /// </summary>
        /// <typeparam name="TModule">The type of the module that receives the event.</typeparam>
        IModuleEventExpectation<TEvent> ForwardsTo<TModule>()
            where TModule : TychoModule;

        /// <summary>
        /// Exposes the expected event to the module's parent.
        /// </summary>
        IModuleEventExpectation<TEvent> Exposes();

        /// <summary>
        /// Maps the expected event to <typeparamref name="TTargetEvent"/> for routed delivery.
        /// </summary>
        /// <typeparam name="TTargetEvent">The target event type.</typeparam>
        /// <param name="map">The event mapper.</param>
        IModuleMappedEventExpectation<TEvent, TTargetEvent> MapsTo<TTargetEvent>(Func<TEvent, TTargetEvent> map)
            where TTargetEvent : class, IEvent;
    }

    /// <summary>
    /// Configures routed delivery for a mapped module event.
    /// </summary>
    /// <typeparam name="TEvent">The type of the expected event.</typeparam>
    /// <typeparam name="TTargetEvent">The target event type.</typeparam>
    public interface IModuleMappedEventExpectation<TEvent, TTargetEvent>
        where TEvent : class, IEvent
        where TTargetEvent : class, IEvent
    {
        /// <summary>
        /// Forwards the mapped event to the module <typeparamref name="TModule"/>.
        /// </summary>
        /// <typeparam name="TModule">The type of the module that receives the mapped event.</typeparam>
        IModuleMappedEventExpectation<TEvent, TTargetEvent> ForwardsTo<TModule>()
            where TModule : TychoModule;

        /// <summary>
        /// Exposes the mapped event to the module's parent.
        /// </summary>
        IModuleMappedEventExpectation<TEvent, TTargetEvent> Exposes();
    }
}
