using System;
using System.Threading.Tasks;
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
using Tycho.Modules;
using Tycho.Structure;
using Tycho.Transactions;
using Tycho.Utils;

namespace Tycho.Apps.Setup
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

        public IAppEventExpectation<TEvent> Expects<TEvent>()
            where TEvent : class, IEvent
        {
            return new AppEventExpectation<TEvent>(this, _registrator);
        }

        public Task BuildAsync()
        {
            IServiceCollection services = _internals.GetServiceCollection();

            if (!_internals.HasService<IOutboxWriter>() || !_internals.HasService<IOutboxConsumer>())
            {
                services.AddSingleton<InMemoryOutbox>()
                        .AddTransient<IOutboxWriter>(sp => sp.GetRequiredService<InMemoryOutbox>())
                        .AddTransient<IOutboxConsumer>(sp => sp.GetRequiredService<InMemoryOutbox>());
            }

            services.AddSingleton<OutboxActivity>();
            services.AddSingleton<OutboxProcessor>();

            if (!_internals.HasService<IInboxWriter>() || !_internals.HasService<IInboxConsumer>())
            {
                services.AddSingleton<InMemoryInbox>()
                        .AddTransient<IInboxWriter>(sp => sp.GetRequiredService<InMemoryInbox>())
                        .AddTransient<IInboxConsumer>(sp => sp.GetRequiredService<InMemoryInbox>());
            }

            services.AddSingleton<InboxActivity>();
            services.AddSingleton<InboxProcessor>();

            if (!_internals.HasService<ITransaction>())
            {
                services.AddScoped<ITransaction, EmptyTransaction>();
            }

            services.AddScoped<IEventBroker, ScopedEventBroker>();
            services.AddTransient<IEventPublisher, EventPublisher>();
            services.AddTransient<IDeliveryStrategy, FinalRouteDelivery>();
            services.AddTransient<IDeliveryStrategy, DownStreamRouteDelivery>();
            services.AddTransient<IPayloadSerializer, JsonPayloadSerializer>();

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

    internal class AppEventExpectation<TEvent> : IAppEventExpectation<TEvent>
        where TEvent : class, IEvent
    {
        private readonly Registrator _registrator;
        private readonly IAppEvents _events;

        public AppEventExpectation(IAppEvents events, Registrator registrator)
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

        public IAppEventExpectation<TEvent> ForwardsTo<TModule>()
            where TModule : TychoModule
        {
            _registrator.ForwardEvent<TEvent, TModule>();
            return this;
        }

        public IAppMappedEventExpectation<TEvent, TTargetEvent> MapsTo<TTargetEvent>(Func<TEvent, TTargetEvent> map)
            where TTargetEvent : class, IEvent
        {
            map.ThrowIfNull();
            return new AppMappedEventExpectation<TEvent, TTargetEvent>(_registrator, map);
        }
    }

    internal class AppMappedEventExpectation<TEvent, TTargetEvent> : IAppMappedEventExpectation<TEvent, TTargetEvent>
        where TEvent : class, IEvent
        where TTargetEvent : class, IEvent
    {
        private readonly Registrator _registrator;
        private readonly Func<TEvent, TTargetEvent> _map;

        public AppMappedEventExpectation(Registrator registrator, Func<TEvent, TTargetEvent> map)
        {
            _registrator = registrator;
            _map = map;
        }

        public IAppMappedEventExpectation<TEvent, TTargetEvent> ForwardsTo<TModule>()
            where TModule : TychoModule
        {
            _registrator.ForwardEvent<TEvent, TTargetEvent, TModule>(_map);
            return this;
        }
    }
}
