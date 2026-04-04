using System;
using Tycho.Events.Routing;
using Tycho.Events.Routing.Steps;
using Tycho.Modules;
using Tycho.Modules.Instance;

namespace Tycho.Events.Registrating.Registrations
{
    internal class ForwardingEventRegistration<TEvent, TModule>
        : RelayEventRegistration<TEvent>
        where TEvent : class, IEvent
        where TModule : TychoModule
    {
        public ForwardingEventRegistration(IModule<TModule> submodule) : base(submodule.EventBroker)
        {
        }

        protected override IRouteStep GetRouteStep() => DownStreamRouteStep.Create<TModule>();
    }

    internal class MappedForwardingEventRegistration<TEvent, TTargetEvent, TModule>
        : MappedRelayEventRegistration<TEvent, TTargetEvent>
        where TEvent : class, IEvent
        where TTargetEvent : class, IEvent
        where TModule : TychoModule
    {
        public MappedForwardingEventRegistration(IModule<TModule> submodule, Func<TEvent, TTargetEvent> map) : base(submodule.EventBroker, map)
        {
        }

        protected override IRouteStep GetRouteStep() => DownStreamRouteStep.Create<TModule>();
    }
}
