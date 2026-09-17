using System;
using Tycho.Events;
using Tycho.Modules;

namespace Tycho.Apps
{
    /// <summary>
    /// An interface for routing events of type <typeparamref name="TEvent"/>.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event to route.</typeparam>
    public interface IEventRouting<TEvent>
        where TEvent : class, IEvent
    {
        /// <summary>
        /// Forwards the event by routing it to the module <typeparamref name="TModule"/>.
        /// </summary>
        /// <typeparam name="TModule">The type of the target module.</typeparam>
        IEventRouting<TEvent> Forwards<TModule>()
            where TModule : TychoModule;

        /// <summary>
        /// Forwards the event by routing it to the module <typeparamref name="TModule"/>, mapped as an event of type <typeparamref name="TTargetEvent"/>.
        /// </summary>
        /// <typeparam name="TTargetEvent">The type of the target event.</typeparam>
        /// <typeparam name="TModule">The type of the target module.</typeparam>
        /// <param name="mapEvent">Maps the original event to the target event.</param>
        /// <exception cref="ArgumentNullException"/>
        IEventRouting<TEvent> ForwardsAs<TTargetEvent, TModule>(Func<TEvent, TTargetEvent> mapEvent)
            where TTargetEvent : class, IEvent
            where TModule : TychoModule;
    }
}
