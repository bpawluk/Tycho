using System;
using Tycho.Events;
using Tycho.Modules;

namespace Tycho.Apps.Routing
{
    /// <summary>
    ///     TODO
    /// </summary>
    public interface IEventRouting<TEvent>
        where TEvent : class, IEvent
    {
        /// <summary>
        ///     TODO
        /// </summary>
        IEventRouting<TEvent> Forwards<Module>()
            where Module : TychoModule;

        /// <summary>
        ///     TODO
        /// </summary>
        IEventRouting<TEvent> ForwardsAs<TTargetEvent, Module>(Func<TEvent, TTargetEvent> map)
            where TTargetEvent : class, IEvent
            where Module : TychoModule;
    }
}