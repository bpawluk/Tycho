using System;
using Tycho.Events;
using Tycho.Modules;

namespace Tycho.Apps
{
    /// <summary>
    /// An interface for routing Events of type <typeparamref name="TEvent"/>.
    /// </summary>
    /// <typeparam name="TEvent">The type of the Event to route.</typeparam>
    public interface IEventRouting<TEvent>
        where TEvent : class, IEvent
    {
        /// <summary>
        /// Forwards the Event by routing it to Module <typeparamref name="TModule"/>.
        /// </summary>
        /// <typeparam name="TModule">The type of the target Module.</typeparam>
        IEventRouting<TEvent> Forwards<TModule>()
            where TModule : TychoModule;

        /// <summary>
        /// Forwards the Event by routing it to Module <typeparamref name="TModule"/>, mapped as an Event of type
        /// <typeparamref name="TTargetEvent"/>.
        /// </summary>
        /// <typeparam name="TTargetEvent">The type of the target Event.</typeparam>
        /// <typeparam name="TModule">The type of the target Module.</typeparam>
        /// <param name="mapEvent">Maps the original Event to the target Event.</param>
        /// <exception cref="ArgumentNullException"/>
        IEventRouting<TEvent> ForwardsAs<TTargetEvent, TModule>(Func<TEvent, TTargetEvent> mapEvent)
            where TTargetEvent : class, IEvent
            where TModule : TychoModule;
    }
}
