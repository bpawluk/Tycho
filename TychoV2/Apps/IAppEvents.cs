using TychoV2.Apps.Routing;
using TychoV2.Events;

namespace TychoV2.Apps
{
    /// <summary>
    /// TODO
    /// </summary>
    public interface IAppEvents
    {
        /// <summary>
        /// TODO
        /// </summary>
        IAppEvents Handles<TEvent, THandler>()
            where TEvent : class, IEvent
            where THandler : class, IHandle<TEvent>;

        /// <summary>
        /// TODO
        /// </summary>
        IEventRouting Routes<TEvent>()
            where TEvent : class, IEvent;
    }
}