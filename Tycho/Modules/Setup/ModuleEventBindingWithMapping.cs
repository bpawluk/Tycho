using System;
using Tycho.Events;
using Tycho.Events.Registrating;

namespace Tycho.Modules.Setup
{
    internal class ModuleEventBindingWithMapping<TEvent, TTargetEvent> : IModuleEventBindingWithMapping<TEvent, TTargetEvent>
        where TEvent : class, IEvent
        where TTargetEvent : class, IEvent
    {
        private readonly Registrator _registrator;
        private readonly Func<TEvent, TTargetEvent> _map;

        public ModuleEventBindingWithMapping(Registrator registrator, Func<TEvent, TTargetEvent> map)
        {
            _registrator = registrator;
            _map = map;
        }

        public IModuleEventBindingWithMapping<TEvent, TTargetEvent> ForwardsTo<TModule>()
            where TModule : TychoModule
        {
            _registrator.ForwardEvent<TEvent, TTargetEvent, TModule>(_map);
            return this;
        }

        public IModuleEventBindingWithMapping<TEvent, TTargetEvent> Exposes()
        {
            _registrator.ExposeEvent(_map);
            return this;
        }
    }
}
