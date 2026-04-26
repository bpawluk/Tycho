using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Events;
using Tycho.Events.Broker;
using Tycho.Events.Delivery;
using Tycho.Events.Delivery.Strategies;
using Tycho.Events.Dispatching;
using Tycho.Events.Inbox;
using Tycho.Events.Inbox.InMemory;
using Tycho.Events.Outbox;
using Tycho.Events.Outbox.InMemory;
using Tycho.Events.Publishing;
using Tycho.Events.Registrating;
using Tycho.Events.Serialization;
using Tycho.Identity.Events;
using Tycho.Structure;

namespace Tycho.Modules.Setup
{
    internal class ModuleEvents : IModuleEvents
    {
        private readonly Internals _internals;
        private readonly Registrator _registrator;

        private IEventBroker? _parentEventBroker;

        public IEventBroker ParentEventBroker => _parentEventBroker ??
            throw new InvalidOperationException("Parent event broker has not been defined yet.");

        public ModuleEvents(Internals internals)
        {
            _internals = internals;
            _registrator = new Registrator(internals);
        }

        public void WithParentEventBroker(IEventBroker parentEventBroker)
        {
            _parentEventBroker = parentEventBroker;
        }

        public IModuleEvents Handles<TEvent, THandler>()
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

        public Task BuildAsync()
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
            services.AddTransient<OutboxProcessorJobFactory>();

            if (!_internals.HasService<IInboxWriter>() || !_internals.HasService<IInboxConsumer>())
            {
                services.AddSingleton<InMemoryInbox>()
                        .AddTransient<IInboxWriter>(sp => sp.GetRequiredService<InMemoryInbox>())
                        .AddTransient<IInboxConsumer>(sp => sp.GetRequiredService<InMemoryInbox>());
            }

            services.AddSingleton<InboxActivity>();
            services.AddSingleton<InboxProcessor>();
            services.AddTransient<InboxProcessorJob>();
            services.AddTransient<InboxProcessorJobFactory>();

            services.AddTransient<IEventBroker, EventBroker>();
            services.AddTransient<IEventPublisher, EventPublisher>();
            services.AddTransient<IEventDispatcher, EventDispatcher>();
            services.AddTransient<IEventHandlerProvider, EventHandlerProvider>();
            services.AddTransient<IDeliveryStrategy, FinalRouteDelivery>();
            services.AddTransient<IDeliveryStrategy, DownStreamRouteDelivery>();
            services.AddTransient<IDeliveryStrategy, UpStreamRouteDelivery>();

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
