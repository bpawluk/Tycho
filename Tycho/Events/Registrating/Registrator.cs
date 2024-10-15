using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Events.Routing;
using Tycho.Events.Routing.Sources;
using Tycho.Modules;
using Tycho.Structure;

namespace Tycho.Events.Registrating
{
    internal class Registrator
    {
        private readonly Internals _internals;

        private IServiceCollection Services => _internals.GetServiceCollection();

        public Registrator(Internals internals)
        {
            _internals = internals;
        }

        public void ExposeEvent<TEvent>()
            where TEvent : class, IEvent
        {
            if (IsSourceAlreadyRegistered<TEvent, UpStreamHandlersSource<TEvent>>())
            {
                throw new ArgumentException($"{typeof(TEvent).Name} is already exposed", nameof(TEvent));
            }

            Services.AddTransient<IHandlersSource, UpStreamHandlersSource<TEvent>>();
        }

        public void ForwardEvent<TEvent, TModule>()
            where TEvent : class, IEvent
            where TModule : TychoModule
        {
            if (IsSourceAlreadyRegistered<TEvent, DownStreamHandlersSource<TEvent, TModule>>())
            {
                throw new ArgumentException($"{typeof(TEvent).Name} is already forwarded to {typeof(TModule).Name}",
                    nameof(TEvent));
            }

            Services.AddTransient<IHandlersSource, DownStreamHandlersSource<TEvent, TModule>>();
        }

        public void HandleEvent<TEvent, THandler>()
            where TEvent : class, IEvent
            where THandler : class, IEventHandler<TEvent>
        {
            if (IsHandlerAlreadyRegistered<TEvent, THandler>())
            {
                throw new ArgumentException($"Event handler for {typeof(TEvent).Name} already registered",
                    nameof(THandler));
            }

            Services.AddTransient<IEventHandler<TEvent>, THandler>();

            if (!IsSourceAlreadyRegistered<TEvent, LocalHandlersSource<TEvent>>())
            {
                Services.AddSingleton<IHandlersSource>(new LocalHandlersSource<TEvent>(_internals));
            }
        }

        private bool IsHandlerAlreadyRegistered<TEvent, THandler>()
            where TEvent : class, IEvent
            where THandler : class, IEventHandler<TEvent>
        {
            return Services.Any(descriptor =>
                descriptor.ServiceType == typeof(IEventHandler<TEvent>) &&
                descriptor.ImplementationType == typeof(THandler));
        }

        private bool IsSourceAlreadyRegistered<TEvent, THandlersSource>()
            where TEvent : class, IEvent
            where THandlersSource : class, IHandlersSource
        {
            return Services.Any(descriptor =>
                descriptor.ServiceType == typeof(IHandlersSource) &&
                descriptor.ImplementationType == typeof(THandlersSource));
        }
    }
}