using System;
using Tycho.Events;
using Tycho.Events.Registrating;
using Tycho.Modules;

namespace Tycho.Apps.Setup
{
    internal class AppEventBindingWithMapping<TEvent, TTargetEvent> : IAppEventBindingWithMapping<TEvent, TTargetEvent>
        where TEvent : class, IEvent
        where TTargetEvent : class, IEvent
    {
        private readonly Registrator _registrator;
        private readonly Func<TEvent, TTargetEvent> _map;

        public AppEventBindingWithMapping(Registrator registrator, Func<TEvent, TTargetEvent> map)
        {
            _registrator = registrator;
            _map = map;
        }

        public IAppEventBindingWithMapping<TEvent, TTargetEvent> ForwardsTo<TModule>()
            where TModule : TychoModule
        {
            _registrator.ForwardEvent<TEvent, TTargetEvent, TModule>(_map);
            return this;
        }
    }
}
