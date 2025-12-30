using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Apps.Routing;
using Tycho.Events;
using Tycho.Events.Inbox;
using Tycho.Events.Inbox.InMemory;
using Tycho.Events.Outbox;
using Tycho.Events.Outbox.InMemory;
using Tycho.Events.Publishing;
using Tycho.Events.Registrating;
using Tycho.Events.Routing;
using Tycho.Events.Routing.Delivery;
using Tycho.Events.Serialization;
using Tycho.Events.Serialization.InMemory;
using Tycho.Registry;
using Tycho.Structure.Internal;

namespace Tycho.Apps.Setup
{
    internal class AppEvents : IAppEvents
    {
        private readonly Internals _internals;
        private readonly Registrator _registrator;
        private readonly IEventHandlerRegistry _handlerRegistry;

        public AppEvents(Internals internals)
        {
            _internals = internals;
            _handlerRegistry = new EventHandlerRegistry(internals);
            _registrator = new Registrator(internals, _handlerRegistry);
        }

        public IAppEvents Handles<TEvent, THandler>()
            where TEvent : class, IEvent
            where THandler : class, IEventHandler<TEvent>
        {
            _registrator.HandleEvent<TEvent, THandler>();
            return this;
        }

        public IEventRouting<TEvent> Routes<TEvent>()
            where TEvent : class, IEvent
        {
            return new EventRouting<TEvent>(_registrator);
        }

        public Task Build()
        {
            var services = _internals.GetServiceCollection();

            if (!_internals.HasService<IPayloadSerializer>())
            {
                services.AddTransient<IPayloadSerializer, InMemoryPayloadSerializer>();
            }

            if (!_internals.HasService<IOutboxWriter>() || !_internals.HasService<IOutboxConsumer>())
            {
                services.AddSingleton<InMemoryOutbox>()
                        .AddTransient<IOutboxWriter>(sp => sp.GetRequiredService<InMemoryOutbox>())
                        .AddTransient<IOutboxConsumer>(sp => sp.GetRequiredService<InMemoryOutbox>());
            }

            services.AddSingleton<OutboxActivity>();
            services.AddSingleton<OutboxProcessor>();
            services.AddTransient<OutboxProcessorJob>();

            if (!_internals.HasService<IInboxWriter>() || !_internals.HasService<IInboxConsumer>())
            {
                services.AddSingleton<InMemoryInbox>()
                        .AddTransient<IInboxWriter>(sp => sp.GetRequiredService<InMemoryInbox>())
                        .AddTransient<IInboxConsumer>(sp => sp.GetRequiredService<InMemoryInbox>());
            }

            services.AddSingleton<InboxActivity>();
            services.AddSingleton<InboxProcessor>();
            services.AddTransient<InboxProcessorJob>();

            services.AddSingleton(_handlerRegistry);
            services.AddTransient<IEventPublisher, EventPublisher>();
            services.AddTransient<IEventRouter, EventRouter>();
            services.AddTransient<IDeliveryStrategyProvider, DeliveryStrategyProvider>();
            services.AddTransient<DownStreamRouteDelivery>();
            services.AddTransient<FinalRouteDelivery>();
            services.AddTransient<UpStreamRouteDelivery>();

            _internals.InternalsBuilt += OnInternalsBuilt;
            return Task.CompletedTask;
        }

        private void OnInternalsBuilt(object _, EventArgs __)
        {
            _internals.GetRequiredService<OutboxProcessor>().Initialize();
            _internals.GetRequiredService<InboxProcessor>().Initialize();
            _internals.InternalsBuilt -= OnInternalsBuilt;
        }
    }
}