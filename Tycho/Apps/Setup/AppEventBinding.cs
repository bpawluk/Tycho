using System;
using Tycho.Events;
using Tycho.Events.Registrating;
using Tycho.Modules;
using Tycho.Utils;

namespace Tycho.Apps.Setup
{
    internal class AppEventBinding<TEvent> : IAppEventBinding<TEvent>
        where TEvent : class, IEvent
    {
        private readonly Registrator _registrator;
        private readonly IAppEvents _events;

        public AppEventBinding(IAppEvents events, Registrator registrator)
        {
            _events = events;
            _registrator = registrator;
        }

        public IAppEvents HandlesWith<THandler>()
            where THandler : class, IEventHandler<TEvent>
        {
            _registrator.HandleEvent<TEvent, THandler>();
            return _events;
        }

        public IAppEventBinding<TEvent> ForwardsTo<TModule>()
            where TModule : TychoModule
        {
            _registrator.ForwardEvent<TEvent, TModule>();
            return this;
        }

        public IAppEventBindingWithMapping<TEvent, TTargetEvent> MapsTo<TTargetEvent>(Func<TEvent, TTargetEvent> map)
            where TTargetEvent : class, IEvent
        {
            map.ThrowIfNull();
            return new AppEventBindingWithMapping<TEvent, TTargetEvent>(_registrator, map);
        }
    }
}
