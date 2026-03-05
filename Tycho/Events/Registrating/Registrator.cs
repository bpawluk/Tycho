using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Events.Handling;
using Tycho.Events.Registrating.Registrations;
using Tycho.Modules;
using Tycho.Modules.Instance;
using Tycho.Structure;
using Tycho.Structure.Parent;

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
            if (IsAlreadyRegistered<TEvent, ExposingEventRegistration<TEvent>>())
            {
                throw new ArgumentException($"{typeof(TEvent).Name} is already exposed", nameof(TEvent));
            }

            Services.AddTransient<IEventRegistration<TEvent>, ExposingEventRegistration<TEvent>>();
        }

        public void ExposeEvent<TEvent, TTargetEvent>(Func<TEvent, TTargetEvent> map)
            where TEvent : class, IEvent
            where TTargetEvent : class, IEvent
        {
            if (IsAlreadyRegistered<TEvent, MappedExposingEventRegistration<TEvent, TTargetEvent>>())
            {
                throw new ArgumentException($"{typeof(TTargetEvent).Name} is already exposed", nameof(TEvent));
            }

            Services.AddTransient<IEventRegistration<TEvent>>(sp => 
                new MappedExposingEventRegistration<TEvent, TTargetEvent>(
                    sp.GetRequiredService<IParentReference>(),
                    map));
        }

        public void ForwardEvent<TEvent, TModule>()
            where TEvent : class, IEvent
            where TModule : TychoModule
        {
            if (IsAlreadyRegistered<TEvent, ForwardingEventRegistration<TEvent, TModule>>())
            {
                throw new ArgumentException(
                    $"{typeof(TEvent).Name} is already forwarded to {typeof(TModule).Name}", 
                    nameof(TEvent));
            }

            Services.AddTransient<IEventRegistration<TEvent>, ForwardingEventRegistration<TEvent, TModule>>();
        }

        public void ForwardEvent<TEvent, TTargetEvent, TModule>(Func<TEvent, TTargetEvent> map)
            where TEvent : class, IEvent
            where TTargetEvent : class, IEvent
            where TModule : TychoModule
        {
            if (IsAlreadyRegistered<TEvent, MappedForwardingEventRegistration<TEvent, TTargetEvent, TModule>>())
            {
                throw new ArgumentException(
                    $"{typeof(TTargetEvent).Name} is already forwarded to {typeof(TModule).Name}",
                    nameof(TEvent));
            }

            Services.AddTransient<IEventRegistration<TEvent>>(sp => 
                new MappedForwardingEventRegistration<TEvent, TTargetEvent, TModule>(
                    sp.GetRequiredService<IModule<TModule>>(),
                    map));
        }

        public void HandleEvent<TEvent, THandler>()
            where TEvent : class, IEvent
            where THandler : class, IEventHandler<TEvent>
        {
            if (IsAlreadyRegistered<TEvent, FinalEventRegistration<TEvent, ScopedEventHandler<TEvent, THandler>>>())
            {
                throw new ArgumentException(
                    $"Event handler for {typeof(TEvent).Name} is already registered",
                    nameof(THandler));
            }

            Services.AddTransient<IEventRegistration<TEvent>, FinalEventRegistration<TEvent, ScopedEventHandler<TEvent, THandler>>>();
            Services.AddTransient<IFinalEventRegistration<TEvent>, FinalEventRegistration<TEvent, ScopedEventHandler<TEvent, THandler>>>();

            Services.AddTransient<ScopedEventHandler<TEvent, THandler>>();
            Services.AddScoped<THandler>();
        }

        private bool IsAlreadyRegistered<TEvent, TRegistration>()
            where TEvent : class, IEvent
            where TRegistration : class, IEventRegistration<TEvent>
        {
            return Services.Any(descriptor =>
                descriptor.ServiceType == typeof(IEventRegistration<TEvent>) &&
                descriptor.ImplementationType == typeof(TRegistration));
        }
    }
}