using System;
using Tycho.Events;
using Tycho.Utils;

namespace Tycho.Modules
{
    /// <summary>
    /// Configures how an expected module event is handled, forwarded, or exposed.
    /// </summary>
    /// <typeparam name="TEvent">The type of the expected event.</typeparam>
    [ReferencedBySourceGenerator]
    public interface IModuleEventBinding<TEvent>
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
        IModuleEventBinding<TEvent> ForwardsTo<TModule>()
            where TModule : TychoModule;

        /// <summary>
        /// Exposes the expected event to the module's parent.
        /// </summary>
        IModuleEventBinding<TEvent> Exposes();

        /// <summary>
        /// Maps the expected event to <typeparamref name="TTargetEvent"/> for routed delivery.
        /// </summary>
        /// <typeparam name="TTargetEvent">The target event type.</typeparam>
        /// <param name="map">The event mapper.</param>
        IModuleEventBindingWithMapping<TEvent, TTargetEvent> MapsTo<TTargetEvent>(Func<TEvent, TTargetEvent> map)
            where TTargetEvent : class, IEvent;
    }
}
