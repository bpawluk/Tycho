using System;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Events;
using Tycho.Events.Broker;
using Tycho.Events.Delivery;
using Tycho.Events.Delivery.Strategies;
using Tycho.Events.Inbox;
using Tycho.Events.Inbox.InMemory;
using Tycho.Events.Outbox;
using Tycho.Events.Outbox.InMemory;
using Tycho.Events.Publishing;
using Tycho.Events.Registrating;
using Tycho.Events.Serialization;
using Tycho.Structure;
using Tycho.Transactions;

namespace Tycho.Modules.Setup
{
    internal class ModuleEvents : IModuleEvents
    {
        private readonly Internals _internals;
        private readonly Registrator _registrator;

        private IEventBroker? _parentEventBroker;

        public IEventBroker ParentEventBroker => _parentEventBroker ?? throw new InvalidOperationException("Parent event broker has not been defined yet.");

        public ModuleEvents(Internals internals)
        {
            _internals = internals;
            _registrator = new Registrator(internals);
        }

        public void WithParentEventBroker(IEventBroker parentEventBroker)
        {
            _parentEventBroker = parentEventBroker;
        }

        public IModuleEventBinding<TEvent> Expects<TEvent>()
            where TEvent : class, IEvent
        {
            return new ModuleEventBinding<TEvent>(this, _registrator);
        }

        public void Build()
        {
            IServiceCollection services = _internals.GetHostBuilder().Services;

            if (!_internals.HasService<IOutboxWriter>() || !_internals.HasService<IOutboxConsumer>())
            {
                services.AddSingleton<InMemoryOutbox>()
                        .AddTransient<IOutboxWriter>(sp => sp.GetRequiredService<InMemoryOutbox>())
                        .AddTransient<IOutboxConsumer>(sp => sp.GetRequiredService<InMemoryOutbox>());
            }

            services.AddSingleton<OutboxActivity>();
            services.AddHostedService<OutboxProcessor>();

            if (!_internals.HasService<IInboxWriter>() || !_internals.HasService<IInboxConsumer>())
            {
                services.AddSingleton<InMemoryInbox>()
                        .AddTransient<IInboxWriter>(sp => sp.GetRequiredService<InMemoryInbox>())
                        .AddTransient<IInboxConsumer>(sp => sp.GetRequiredService<InMemoryInbox>());
            }

            services.AddSingleton<InboxActivity>();
            services.AddHostedService<InboxProcessor>();

            if (!_internals.HasService<ITransaction>())
            {
                services.AddScoped<ITransaction, EmptyTransaction>();
            }

            services.AddScoped<IEventBroker, ScopedEventBroker>();
            services.AddTransient<IEventPublisher, EventPublisher>();
            services.AddTransient<IDeliveryStrategy, FinalRouteDelivery>();
            services.AddTransient<IDeliveryStrategy, DownStreamRouteDelivery>();
            services.AddTransient<IDeliveryStrategy, UpStreamRouteDelivery>();
            services.AddTransient<IPayloadSerializer, JsonPayloadSerializer>();
        }
    }
}
