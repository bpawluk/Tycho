using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Events.Handling;
using Tycho.Events.Routing;
using Tycho.Events.Routing.Sources;
using Tycho.Modules;
using Tycho.Structure;
using Tycho.Structure.Data;

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
                throw new ArgumentException(
                    $"{typeof(TEvent).Name} is already exposed", 
                    nameof(TEvent));
            }

            Services.AddTransient<IHandlersSource, UpStreamHandlersSource<TEvent>>();
        }

        public void ExposeEvent<TEvent, TTargetEvent>(Func<TEvent, TTargetEvent> map)
            where TEvent : class, IEvent
            where TTargetEvent : class, IEvent
        {
            if (IsSourceAlreadyRegistered<TEvent, UpStreamMappedHandlersSource<TEvent, TTargetEvent>>())
            {
                throw new ArgumentException(
                    $"{typeof(TEvent).Name} is already exposed",
                    nameof(TEvent));
            }

            Services.AddTransient<IHandlersSource>(
                sp => new UpStreamMappedHandlersSource<TEvent, TTargetEvent>(
                    sp.GetRequiredService<IParent>(),
                    map));
        }

        public void ForwardEvent<TEvent, TModule>()
            where TEvent : class, IEvent
            where TModule : TychoModule
        {
            if (IsSourceAlreadyRegistered<TEvent, DownStreamHandlersSource<TEvent, TModule>>())
            {
                throw new ArgumentException(
                    $"{typeof(TEvent).Name} is already forwarded to {typeof(TModule).Name}",
                    nameof(TEvent));
            }

            Services.AddTransient<IHandlersSource, DownStreamHandlersSource<TEvent, TModule>>();
        }

        public void ForwardEvent<TEvent, TTargetEvent, TModule>(Func<TEvent, TTargetEvent> map)
            where TEvent : class, IEvent
            where TTargetEvent : class, IEvent
            where TModule : TychoModule
        {
            if (IsSourceAlreadyRegistered<TEvent, DownStreamMappedHandlersSource<TEvent, TTargetEvent, TModule>>())
            {
                throw new ArgumentException(
                    $"{typeof(TEvent).Name} is already forwarded to {typeof(TModule).Name}",
                    nameof(TEvent));
            }

            Services.AddTransient<IHandlersSource>(
                sp => new DownStreamMappedHandlersSource<TEvent, TTargetEvent, TModule>(
                    sp.GetRequiredService<IModule<TModule>>(),
                    map));
        }

        public void HandleEvent<TEvent, THandler>()
            where TEvent : class, IEvent
            where THandler : class, IEventHandler<TEvent>
        {
            if (IsHandlerAlreadyRegistered<TEvent, ScopedEventHandler<TEvent, THandler>>())
            {
                throw new ArgumentException(
                    $"Event handler for {typeof(TEvent).Name} already registered",
                    nameof(THandler));
            }

            Services.AddTransient<IEventHandler<TEvent>, ScopedEventHandler<TEvent, THandler>>();
            Services.AddScoped<THandler>();

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