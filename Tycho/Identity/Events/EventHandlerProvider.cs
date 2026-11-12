using System;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Events;

namespace Tycho.Identity.Events
{
    internal class EventHandlerProvider : IEventHandlerProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public EventHandlerProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IEventHandler<TEvent> GetHandler<TEvent>(EventHandlerIdentity handlerId) where TEvent : class, IEvent
        {
            return _serviceProvider.GetKeyedService<IEventHandler<TEvent>>(handlerId)
                ?? throw new ArgumentException($"Event handler with identity '{handlerId}' is not registered for '{typeof(TEvent).Name}' event.", nameof(handlerId));
        }
    }
}
