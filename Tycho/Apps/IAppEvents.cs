using Tycho.Apps.Routing;
using Tycho.Events;
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
        /// Declares that the application will handle all events of type <typeparamref name="TEvent"/>
        /// using handler <typeparamref name="THandler"/>.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event to handle.</typeparam>
        /// <typeparam name="THandler">The type of event handler.</typeparam>
        [ReferencedBySourceGenerator]
        IAppEvents Handles<TEvent, THandler>()
            where TEvent : class, IEvent
            where THandler : class, IEventHandler<TEvent>;

        /// <summary>
        /// Declares that the application will route all events of type <typeparamref name="TEvent"/>
        /// according to the configuration defined using the returned instance of
        /// <see cref="IEventRouting{TEvent}"/>.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event to route.</typeparam>
        /// <returns>An instance of <see cref="IEventRouting{TEvent}"/> to configure event routing.</returns>
        IEventRouting<TEvent> Routes<TEvent>()
            where TEvent : class, IEvent;
    }
}
