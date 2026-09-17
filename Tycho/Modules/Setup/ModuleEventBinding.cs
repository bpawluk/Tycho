using System;
using Tycho.Events;
using Tycho.Events.Registrating;
using Tycho.Utils;

namespace Tycho.Modules.Setup
{
    internal class ModuleEventBinding<TEvent> : IModuleEventBinding<TEvent>
        where TEvent : class, IEvent
    {
        private readonly Registrator _registrator;
        private readonly IModuleEvents _events;

        public ModuleEventBinding(IModuleEvents events, Registrator registrator)
        {
            _events = events;
            _registrator = registrator;
        }

        public IModuleEvents HandlesWith<THandler>()
            where THandler : class, IEventHandler<TEvent>
        {
            _registrator.HandleEvent<TEvent, THandler>();
            return _events;
        }

        public IModuleEventBinding<TEvent> ForwardsTo<TModule>()
            where TModule : TychoModule
        {
            _registrator.ForwardEvent<TEvent, TModule>();
            return this;
        }

        public IModuleEventBinding<TEvent> Exposes()
        {
            _registrator.ExposeEvent<TEvent>();
            return this;
        }

        public IModuleEventBindingWithMapping<TEvent, TTargetEvent> MapsTo<TTargetEvent>(Func<TEvent, TTargetEvent> map)
            where TTargetEvent : class, IEvent
        {
            map.ThrowIfNull();
            return new ModuleEventBindingWithMapping<TEvent, TTargetEvent>(_registrator, map);
        }
    }
}
