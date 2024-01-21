using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Handlers;
using Tycho.Messaging.Interceptors;
using Tycho.Messaging.Payload;

namespace Tycho.Messaging.Forwarders
{
    internal abstract class EventForwarder<EventIn, EventOut>
        : ForwarderBase<EventIn, EventOut>
        , IEventHandler<EventIn>
        where EventIn : class, IEvent
        where EventOut : class, IEvent
    {
        private readonly IEventInterceptor<EventOut>? _interceptor;

        public EventForwarder(
            IModule target,
            Func<EventIn, EventOut> mapping,
            IEventInterceptor<EventOut>? interceptor)
            : base(target, mapping)
        {
            _interceptor = interceptor;
        }

        public async Task Handle(EventIn eventData, CancellationToken cancellationToken)
        {
            var mappedEvent = _messageMapping(eventData);

            if (_interceptor != null)
            {
                await _interceptor.ExecuteBefore(mappedEvent, cancellationToken).ConfigureAwait(false);
            }

            _target.Publish(mappedEvent, cancellationToken);

            if (_interceptor != null)
            {
                await _interceptor.ExecuteAfter(mappedEvent, cancellationToken).ConfigureAwait(false);
            }
        }
    }

    internal class EventUpForwarder<EventIn, EventOut>
        : EventForwarder<EventIn, EventOut>
        , IEventHandler<EventIn>
        where EventIn : class, IEvent
        where EventOut : class, IEvent
    {
        public EventUpForwarder(
            IModule module,
            Func<EventIn, EventOut> mapping,
            IEventInterceptor<EventOut>? interceptor = null)
            : base(module, mapping, interceptor) { }
    }

    internal class EventDownForwarder<EventIn, EventOut, Module>
        : EventForwarder<EventIn, EventOut>
        , IEventHandler<EventIn>
        where EventIn : class, IEvent
        where EventOut : class, IEvent
        where Module : TychoModule
    {
        public EventDownForwarder(
            IModule<Module> submodule,
            Func<EventIn, EventOut> mapping,
            IEventInterceptor<EventOut>? interceptor = null)
            : base(submodule, mapping, interceptor) { }
    } 
}
