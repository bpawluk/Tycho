using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using TychoV2.Events.Handling;
using TychoV2.Modules;
using TychoV2.Structure;

namespace TychoV2.Events.Registrating
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
            Services.AddTransient<IHandlersSource<TEvent>, UpStreamHandlersSource<TEvent>>();
        }

        public void ForwardEvent<TEvent, TModule>()
           where TEvent : class, IEvent
           where TModule : TychoModule
        {
            if (IsSourceAlreadyRegistered<TEvent, DownStreamHandlersSource<TEvent, TModule>>())
            {
                throw new ArgumentException($"{typeof(TEvent).Name} is already exposed", nameof(TEvent));
            }
            Services.AddTransient<IHandlersSource<TEvent>, DownStreamHandlersSource<TEvent, TModule>>();
        }

        public void HandleEvent<TEvent, THandler>()
            where TEvent : class, IEvent
            where THandler : class, IHandle<TEvent>
        {
            if (IsHandlerAlreadyRegistered<TEvent, THandler>())
            {
                throw new ArgumentException($"Event handler for {typeof(TEvent).Name} already registered", nameof(THandler));
            }

            Services.AddTransient<IHandle<TEvent>, THandler>();

            if (!IsSourceAlreadyRegistered<TEvent, LocalHandlersSource<TEvent>>())
            {
                Services.AddTransient<IHandlersSource<TEvent>, LocalHandlersSource<TEvent>>();
            }
        }

        private bool IsHandlerAlreadyRegistered<TEvent, THandler>()
            where TEvent : class, IEvent
            where THandler : class, IHandle<TEvent>
        {
            return Services.Any(descriptor =>
                descriptor.ServiceType == typeof(IHandle<TEvent>) &&
                descriptor.ImplementationType == typeof(THandler));
        }

        private bool IsSourceAlreadyRegistered<TEvent, THandlersSource>()
            where TEvent : class, IEvent
            where THandlersSource : class, IHandlersSource<TEvent>
        {
            return Services.Any(descriptor =>
                descriptor.ServiceType == typeof(IHandlersSource<TEvent>) &&
                descriptor.ImplementationType == typeof(THandlersSource));
        }
    }
}
