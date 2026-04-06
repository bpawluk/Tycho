using System;
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
            if (!TryAddRegistration<TEvent, ExposingEventRegistration<TEvent>>())
            {
                throw new ArgumentException($"{typeof(TEvent).Name} is already exposed", nameof(TEvent));
            }
        }

        public void ExposeEvent<TEvent, TTargetEvent>(Func<TEvent, TTargetEvent> map)
            where TEvent : class, IEvent
            where TTargetEvent : class, IEvent
        {
            if (!TryAddRegistration<TEvent, MappedExposingEventRegistration<TEvent, TTargetEvent>>(sp =>
                new MappedExposingEventRegistration<TEvent, TTargetEvent>(
                    sp.GetRequiredService<IParentReference>(),
                    map)))
            {
                throw new ArgumentException($"{typeof(TTargetEvent).Name} is already exposed", nameof(TEvent));
            }
        }

        public void ForwardEvent<TEvent, TModule>()
            where TEvent : class, IEvent
            where TModule : TychoModule
        {
            if (!TryAddRegistration<TEvent, ForwardingEventRegistration<TEvent, TModule>>())
            {
                throw new ArgumentException(
                    $"{typeof(TEvent).Name} is already forwarded to {typeof(TModule).Name}",
                    nameof(TEvent));
            }
        }

        public void ForwardEvent<TEvent, TTargetEvent, TModule>(Func<TEvent, TTargetEvent> map)
            where TEvent : class, IEvent
            where TTargetEvent : class, IEvent
            where TModule : TychoModule
        {
            if (!TryAddRegistration<TEvent, MappedForwardingEventRegistration<TEvent, TTargetEvent, TModule>>(sp =>
                new MappedForwardingEventRegistration<TEvent, TTargetEvent, TModule>(
                    sp.GetRequiredService<IModule<TModule>>(),
                    map)))
            {
                throw new ArgumentException(
                    $"{typeof(TTargetEvent).Name} is already forwarded to {typeof(TModule).Name}",
                    nameof(TEvent));
            }
        }

        public void HandleEvent<TEvent, THandler>()
            where TEvent : class, IEvent
            where THandler : class, IEventHandler<TEvent>
        {
            if (!TryAddFinalRegistration<TEvent, FinalEventRegistration<TEvent, ScopedEventHandler<TEvent, THandler>>>())
            {
                throw new ArgumentException(
                    $"Event handler for {typeof(TEvent).Name} is already registered",
                    nameof(THandler));
            }

            Services.AddTransient<ScopedEventHandler<TEvent, THandler>>();
            Services.AddScoped<THandler>();
        }

        private bool TryAddRegistration<TEvent, TRegistration>()
            where TEvent : class, IEvent
            where TRegistration : class, IEventRegistration<TEvent>
        {
            if (_internals.HasService<TRegistration>())
            {
                return false;
            }

            Services.AddTransient<TRegistration>();
            Services.AddTransient<IEventRegistration<TEvent>>(sp => sp.GetRequiredService<TRegistration>());
            return true;
        }

        private bool TryAddRegistration<TEvent, TRegistration>(Func<IServiceProvider, TRegistration> implementationFactory)
            where TEvent : class, IEvent
            where TRegistration : class, IEventRegistration<TEvent>
        {
            if (_internals.HasService<TRegistration>())
            {
                return false;
            }

            Services.AddTransient(implementationFactory);
            Services.AddTransient<IEventRegistration<TEvent>>(sp => sp.GetRequiredService<TRegistration>());
            return true;
        }

        private bool TryAddFinalRegistration<TEvent, TRegistration>()
            where TEvent : class, IEvent
            where TRegistration : class, IFinalEventRegistration<TEvent>
        {
            if (_internals.HasService<TRegistration>())
            {
                return false;
            }

            Services.AddTransient<TRegistration>();
            Services.AddTransient<IEventRegistration<TEvent>>(sp => sp.GetRequiredService<TRegistration>());
            Services.AddTransient<IFinalEventRegistration<TEvent>>(sp => sp.GetRequiredService<TRegistration>());
            return true;
        }
    }
}