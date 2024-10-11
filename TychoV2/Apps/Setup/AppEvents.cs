using Microsoft.Extensions.DependencyInjection;
using TychoV2.Apps.Routing;
using TychoV2.Events;
using TychoV2.Events.Broker;
using TychoV2.Events.Registrating;
using TychoV2.Persistence;
using TychoV2.Structure;

namespace TychoV2.Apps.Setup
{
    internal class AppEvents : IAppEvents
    {
        private readonly Registrator _registrator;

        public AppEvents(Internals internals)
        {
            _registrator = new Registrator(internals);

            internals.GetServiceCollection()
                .AddSingleton(sp => new EventBroker(internals, sp.GetRequiredService<IOutbox>()))
                .AddSingleton<IPublish>(sp => sp.GetRequiredService<EventBroker>())
                .AddSingleton<IEventProcessor>(sp => sp.GetRequiredService<EventBroker>());
        }

        public IAppEvents Handles<TEvent, THandler>()
            where TEvent : class, IEvent
            where THandler : class, IHandle<TEvent>
        {
            _registrator.HandleEvent<TEvent, THandler>();
            return this;
        }

        public IEventRouting Routes<TEvent>()
            where TEvent : class, IEvent
        {
            return new EventRouting<TEvent>(_registrator);
        }
    }
}
