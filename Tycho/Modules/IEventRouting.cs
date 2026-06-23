using System;
using Tycho.Events;

namespace Tycho.Modules
{
    /// <summary>
    /// An interface for routing Events of type <typeparamref name="TEvent"/>.
    /// </summary>
    /// <typeparam name="TEvent">The type of the Event to route.</typeparam>
    public interface IEventRouting<TEvent>
        where TEvent : class, IEvent
    {
        /// <summary>
        /// Exposes the Event by routing it to the Module's parent.
        /// </summary>
        IEventRouting<TEvent> Exposes();

        /// <summary>
        /// Exposes the Event by routing it to the Module's parent, mapped as an Event of type
        /// <typeparamref name="TTargetEvent"/>.
        /// </summary>
        /// <typeparam name="TTargetEvent">The type of the target Event.</typeparam>
        /// <param name="mapEvent">Maps the original Event to the target Event.</param>
        /// <exception cref="ArgumentNullException"/>
        IEventRouting<TEvent> ExposesAs<TTargetEvent>(Func<TEvent, TTargetEvent> mapEvent)
            where TTargetEvent : class, IEvent;

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
