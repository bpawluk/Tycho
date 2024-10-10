using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using TychoV2.Events.Routing;
using TychoV2.Structure;

namespace TychoV2.Events.Handling
{
    internal class LocalHandlersSource<TEvent> : IHandlersSource<TEvent>
        where TEvent : class, IEvent
    {
        private readonly Internals _internals;

        public LocalHandlersSource(Internals internals)
        {
            _internals = internals;
        }

        public IReadOnlyCollection<HandlerIdentity> IdentifyHandlers() => _internals
            .GetServices<IHandle<TEvent>>()
            .Select(handler => new HandlerIdentity(
                _internals.Identifier,
                handler.GetType().FullName))
            .ToArray();

        public IHandle<TEvent>? FindHandler(HandlerIdentity handlerIdentity)
        {
            if (handlerIdentity.SourceId == _internals.Identifier)
            {
                var handler = _internals
                    .GetServices<IHandle<TEvent>>()
                    .FirstOrDefault(handler => handlerIdentity.HandlerId == handler.GetType().FullName);
                return handler ?? throw new InvalidOperationException($"Missing {handlerIdentity} event handler");
            }
            return null;
        }
    }
}
