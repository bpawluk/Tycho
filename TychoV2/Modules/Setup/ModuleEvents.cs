using Microsoft.Extensions.DependencyInjection;
using System;
using TychoV2.Events;
using TychoV2.Events.Broker;
using TychoV2.Events.Routing;
using TychoV2.Modules.Routing;
using TychoV2.Persistence;
using TychoV2.Structure;

namespace TychoV2.Modules.Setup
{
    internal class ModuleEvents : IModuleEvents
    {
        private readonly Internals _internals;

        private IEventRouter? _parentEventRouter;

        public IEventRouter ParentEventRouter =>
            _parentEventRouter ??
            throw new InvalidOperationException("Parent event router has not been defined yet.");

        public ModuleEvents(Internals internals)
        {
            _internals = internals;
            _internals.GetServiceCollection()
                .AddSingleton(sp => new EventBroker(_internals, sp.GetRequiredService<IOutbox>()))
                .AddSingleton<IPublish>(sp => sp.GetRequiredService<EventBroker>())
                .AddSingleton<IEventProcessor>(sp => sp.GetRequiredService<EventBroker>());
        }

        public void WithParentEventRouter(IEventRouter parentEventRouter)
        {
            _parentEventRouter = parentEventRouter;
        }

        public IModuleEvents Handles<TEvent, THandler>()
            where TEvent : class, IEvent
            where THandler : class, IHandle<TEvent>
        {
            throw new System.NotImplementedException();
        }

        public IEventRouting Routes<TEvent>()
            where TEvent : class, IEvent
        {
            throw new System.NotImplementedException();
        }
    }
}
