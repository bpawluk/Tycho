using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Tycho.Events;
using Tycho.Events.Publishing;
using Tycho.Events.Registrating;
using Tycho.Events.Routing;
using Tycho.Modules.Routing;
using Tycho.Persistence;
using Tycho.Persistence.InMemory;
using Tycho.Persistence.Processing;
using Tycho.Structure.Internal;

namespace Tycho.Modules.Setup
{
    internal class ModuleEvents : IModuleEvents
    {
        private readonly Internals _internals;
        private readonly Registrator _registrator;

        private IEventRouter? _parentEventRouter;

        public IEventRouter ParentEventRouter => _parentEventRouter ??
            throw new InvalidOperationException("Parent event router has not been defined yet.");

        public ModuleEvents(Internals internals)
        {
            _internals = internals;
            _registrator = new Registrator(internals);
        }

        public void WithParentEventRouter(IEventRouter parentEventRouter)
        {
            _parentEventRouter = parentEventRouter;
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

        public Task Build()
        {
            var services = _internals.GetServiceCollection();

            if (!_internals.HasService<IOutboxWriter>() || !_internals.HasService<IOutboxConsumer>())
            {
                services.AddSingleton<InMemoryOutbox>()
                        .AddTransient<IOutboxWriter>(sp => sp.GetRequiredService<InMemoryOutbox>())
                        .AddTransient<IOutboxConsumer>(sp => sp.GetRequiredService<InMemoryOutbox>())
                        .AddTransient<IPayloadSerializer, InMemoryPayloadSerializer>();
            }

            services.AddSingleton<OutboxProcessor>()
                    .AddSingleton<OutboxActivity>()
                    .AddTransient<IEntryProcessor, EntryProcessor>()
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