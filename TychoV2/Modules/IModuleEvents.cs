using TychoV2.Events;
using TychoV2.Modules.Routing;

namespace TychoV2.Modules
{
    /// <summary>
    /// TODO
    /// </summary>
    public interface IModuleEvents
    {
        /// <summary>
        /// TODO
        /// </summary>
        IModuleEvents Handles<TEvent, THandler>()
            where TEvent : class, IEvent
            where THandler : class, IEventHandler<TEvent>;

        /// <summary>
        /// TODO
        /// </summary>
        IEventRouting Routes<TEvent>()
            where TEvent : class, IEvent;
    }
}