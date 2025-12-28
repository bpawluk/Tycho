using System;
using Tycho.Events;
using Tycho.Events.Registrating;
using Tycho.Modules;

namespace Tycho.Apps.Routing
{
    internal class EventRouting<TEvent> : IEventRouting<TEvent>
        where TEvent : class, IEvent
    {
        private readonly Registrator _registrator;

        public EventRouting(Registrator registrator)
        {
            _registrator = registrator;
        }

        public IEventRouting<TEvent> Forwards<TModule>()
            where TModule : TychoModule
        {
            _registrator.ForwardEvent<TEvent, TModule>();
            return this;
        }

        public IEventRouting<TEvent> ForwardsAs<TTargetEvent, TModule>(Func<TEvent, TTargetEvent> map)
            where TTargetEvent : class, IEvent
            where TModule : TychoModule
        {
            _registrator.ForwardEvent<TEvent, TTargetEvent, TModule>(map);
            return this;
        }
    }
}