using Tycho.Events;
using Tycho.Modules.Routing;

namespace Tycho.Modules
{
    /// <summary>
    ///     TODO
    /// </summary>
    public interface IModuleEvents
    {
        /// <summary>
        ///     TODO
        /// </summary>
        IModuleEvents Handles<TEvent, THandler>()
            where TEvent : class, IEvent
            where THandler : class, IEventHandler<TEvent>;

        /// <summary>
        ///     TODO
        /// </summary>
        IEventRouting<TEvent> Routes<TEvent>()
            where TEvent : class, IEvent;
    }
}