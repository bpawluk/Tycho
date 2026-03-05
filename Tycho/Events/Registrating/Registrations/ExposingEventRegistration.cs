using System;
using Tycho.Events.Routing.Routes;
using Tycho.Structure.Parent;

namespace Tycho.Events.Registrating.Registrations
{
    internal class ExposingEventRegistration<TEvent>
        : RelayEventRegistration<TEvent>
        where TEvent : class, IEvent
    {
        public ExposingEventRegistration(IParentReference parent)
            : base(parent.EventBroker)
        {
        }

        protected override IRouteStep GetRouteStep() => UpStreamRouteStep.Create();
    }

    internal class MappedExposingEventRegistration<TEvent, TTargetEvent>
        : MappedRelayEventRegistration<TEvent, TTargetEvent>
        where TEvent : class, IEvent
        where TTargetEvent : class, IEvent
    {
        public MappedExposingEventRegistration(IParentReference parent, Func<TEvent, TTargetEvent> map)
            : base(parent.EventBroker, map)
        {
        }

        protected override IRouteStep GetRouteStep() => UpStreamRouteStep.Create();
    }
}
