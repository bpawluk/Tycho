using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using TychoV2.Apps.Routing;
using TychoV2.Events;
using TychoV2.Events.Broker;
using TychoV2.Events.Registrating;
using TychoV2.Events.Routing;
using TychoV2.Persistence;
using TychoV2.Persistence.InMemory;
using TychoV2.Persistence.Processing;
using TychoV2.Structure;

namespace TychoV2.Apps.Setup
{
    internal class AppEvents : IAppEvents
    {
        private readonly Internals _internals;
        private readonly Registrator _registrator;

        public AppEvents(Internals internals)
        {
            _internals = internals;
            _registrator = new Registrator(internals);
        }

        public IAppEvents Handles<TEvent, THandler>()
            where TEvent : class, IEvent
            where THandler : class, IEventHandler<TEvent>
        {
            _registrator.HandleEvent<TEvent, THandler>();
            return this;
        }

        public IEventRouting Routes<TEvent>()
            where TEvent : class, IEvent
        {
            return new EventRouting<TEvent>(_registrator);
        }

        public Task Build()
        {
            _internals.GetServiceCollection()
                .AddSingleton<OutboxProcessor>()
                .AddSingleton<OutboxProcessorSettings>()
                .AddSingleton<IOutbox, InMemoryOutbox>()
                .AddTransient<IPayloadSerializer, InMemoryPayloadSerializer>()
                .AddTransient<IEventRouter, EventRouter>()
                .AddTransient<IEventPublisher, EventPublisher>();
            _internals.InternalsBuilt += OnInternalsBuilt;
            return Task.CompletedTask;
        }

        private void OnInternalsBuilt(object _, EventArgs __)
        {
            _internals.GetRequiredService<OutboxProcessor>().Initialize();
            _internals.InternalsBuilt -= OnInternalsBuilt;
        }
    }
}
