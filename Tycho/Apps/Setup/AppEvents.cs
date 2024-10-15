﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Apps.Routing;
using Tycho.Events;
using Tycho.Events.Publishing;
using Tycho.Events.Registrating;
using Tycho.Events.Routing;
using Tycho.Persistence;
using Tycho.Persistence.InMemory;
using Tycho.Persistence.Processing;
using Tycho.Structure;

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
                .AddTransient<IEntryProcessor, EntryProcessor>()
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