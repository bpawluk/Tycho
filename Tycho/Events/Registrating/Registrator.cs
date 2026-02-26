using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Events.Routing.Sources;
using Tycho.Modules;
using Tycho.Modules.Instance;
using Tycho.Registry;
using Tycho.Structure.External;
using Tycho.Structure.Internal;

namespace Tycho.Events.Registrating
{
    internal class Registrator
    {
        private readonly Internals _internals;
        private readonly IEventHandlerRegistry _handlerRegistry;

        private IServiceCollection Services => _internals.GetServiceCollection();

        public Registrator(Internals internals, IEventHandlerRegistry handlerRegistry)
        {
            _internals = internals;
            _handlerRegistry = handlerRegistry;
        }

        public void ExposeEvent<TEvent>()
            where TEvent : class, IEvent
        {
            if (IsSourceAlreadyRegistered<TEvent, UpStreamRouteSource<TEvent>>())
            {
                throw new ArgumentException(
                    $"{typeof(TEvent).Name} is already exposed", 
                    nameof(TEvent));
            }

            Services.AddTransient<IRouteSource<TEvent>, UpStreamRouteSource<TEvent>>();
        }

        public void ExposeEvent<TEvent, TTargetEvent>(Func<TEvent, TTargetEvent> map)
            where TEvent : class, IEvent
            where TTargetEvent : class, IEvent
        {
            if (IsSourceAlreadyRegistered<TEvent, UpStreamMappedRouteSource<TEvent, TTargetEvent>>())
            {
                throw new ArgumentException(
                    $"{typeof(TEvent).Name} is already exposed",
                    nameof(TEvent));
            }

            Services.AddTransient<IRouteSource<TEvent>>(
                sp => new UpStreamMappedRouteSource<TEvent, TTargetEvent>(
                    sp.GetRequiredService<IParent>(),
                    map));
        }

        public void ForwardEvent<TEvent, TModule>()
            where TEvent : class, IEvent
            where TModule : TychoModule
        {
            if (IsSourceAlreadyRegistered<TEvent, DownStreamRouteSource<TEvent, TModule>>())
            {
                throw new ArgumentException(
                    $"{typeof(TEvent).Name} is already forwarded to {typeof(TModule).Name}",
                    nameof(TEvent));
            }

            Services.AddTransient<IRouteSource<TEvent>, DownStreamRouteSource<TEvent, TModule>>();
        }

        public void ForwardEvent<TEvent, TTargetEvent, TModule>(Func<TEvent, TTargetEvent> map)
            where TEvent : class, IEvent
            where TTargetEvent : class, IEvent
            where TModule : TychoModule
        {
            if (IsSourceAlreadyRegistered<TEvent, DownStreamMappedRouteSource<TEvent, TTargetEvent, TModule>>())
            {
                throw new ArgumentException(
                    $"{typeof(TEvent).Name} is already forwarded to {typeof(TModule).Name}",
                    nameof(TEvent));
            }

            Services.AddTransient<IRouteSource<TEvent>>(
                sp => new DownStreamMappedRouteSource<TEvent, TTargetEvent, TModule>(
                    sp.GetRequiredService<IModule<TModule>>(),
                    map));
        }

        public void HandleEvent<TEvent, THandler>()
            where TEvent : class, IEvent
            where THandler : class, IEventHandler<TEvent>
        {
            if (IsHandlerAlreadyRegistered<TEvent, THandler>())
            {
                throw new ArgumentException(
                    $"Event handler for {typeof(TEvent).Name} is already registered",
                    nameof(THandler));
            }

            Services.AddScoped<THandler>();
            _handlerRegistry.RegisterHandler<TEvent, THandler>();

            if (!IsSourceAlreadyRegistered<TEvent, LocalRouteSource<TEvent>>())
            {
                Services.AddTransient<IRouteSource<TEvent>, LocalRouteSource<TEvent>>();
            }
        }

        private bool IsHandlerAlreadyRegistered<TEvent, THandler>()
            where TEvent : class, IEvent
            where THandler : class, IEventHandler<TEvent>
        {
            return Services.Any(descriptor => 
                descriptor.ServiceType == typeof(THandler));
        }

        private bool IsSourceAlreadyRegistered<TEvent, TRouteSource>()
            where TEvent : class, IEvent
            where TRouteSource : class, IRouteSource<TEvent>
        {
            return Services.Any(descriptor =>
                descriptor.ServiceType == typeof(IRouteSource<TEvent>) &&
                descriptor.ImplementationType == typeof(TRouteSource));
        }
    }
}