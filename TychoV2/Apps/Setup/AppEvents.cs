using Microsoft.Extensions.DependencyInjection;
using TychoV2.Apps.Routing;
using TychoV2.Events;
using TychoV2.Events.Broker;
using TychoV2.Persistence;
using TychoV2.Structure;

namespace TychoV2.Apps.Setup
{
    internal class AppEvents : IAppEvents
    {
        private readonly Internals _internals;

        public AppEvents(Internals internals)
        {
            _internals = internals;
            _internals.GetServiceCollection()
                .AddSingleton(sp => new EventBroker(_internals, sp.GetRequiredService<IOutbox>()))
                .AddSingleton<IPublish>(sp => sp.GetRequiredService<EventBroker>())
                .AddSingleton<IEventProcessor>(sp => sp.GetRequiredService<EventBroker>());
        }

        public IAppEvents Handles<TEvent, THandler>()
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
