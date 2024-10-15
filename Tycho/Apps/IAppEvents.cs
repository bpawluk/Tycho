using Tycho.Apps.Routing;
using Tycho.Events;

namespace Tycho.Apps
{
    /// <summary>
    ///     TODO
    /// </summary>
    public interface IAppEvents
    {
        /// <summary>
        ///     TODO
        /// </summary>
        IAppEvents Handles<TEvent, THandler>()
            where TEvent : class, IEvent
            where THandler : class, IEventHandler<TEvent>;

        /// <summary>
        ///     TODO
        /// </summary>
        IEventRouting Routes<TEvent>()
            where TEvent : class, IEvent;
    }
}