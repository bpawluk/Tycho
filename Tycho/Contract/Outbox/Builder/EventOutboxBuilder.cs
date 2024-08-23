using System;
using Tycho.Messaging.Forwarders;
using Tycho.Messaging.Handlers;
using Tycho.Messaging.Payload;

namespace Tycho.Contract.Outbox.Builder
{
    internal partial class OutboxBuilder : IOutboxConsumer, IEventOutboxConsumer, IRequestOutboxConsumer, IOutboxDefinition, IEventOutboxDefinition, IRequestOutboxDefinition
    {
        IOutboxDefinition IEventOutboxDefinition.Declare<Event>()
        {
            AddMessageDefinition(typeof(Event), nameof(Event));
            return this;
        }

        IOutboxConsumer IEventOutboxConsumer.Handle<Event>(IEventHandler<Event> handler)
        {
            RegisterEventHandler(handler);
            return this;
        }

        IOutboxConsumer IEventOutboxConsumer.Handle<Event, Handler>()
        {
            Func<Handler> handlerCreator = () => _instanceCreator.CreateInstance<Handler>();
            var handler = new TransientEventHandler<Event>(handlerCreator);
            RegisterEventHandler(handler);
            return this;
        }

        IOutboxConsumer IEventOutboxConsumer.Forward<Event, Module>()
        {
            Func<Event, Event> mapping = eventData => eventData;
            Func<IEventHandler<Event>> handlerCreator = () =>
            {
                return _instanceCreator.CreateInstance<EventDownForwarder<Event, Event, Module>>(mapping);
            };
            var handler = new TransientEventHandler<Event>(handlerCreator);
            RegisterEventHandler(handler);
            return this;
        }

        IOutboxConsumer IEventOutboxConsumer.Forward<EventIn, EventOut, Module>(Func<EventIn, EventOut> eventMapping)
        {
            Func<IEventHandler<EventIn>> handlerCreator = () =>
            {
                return _instanceCreator.CreateInstance<EventDownForwarder<EventIn, EventOut, Module>>(eventMapping);
            };
            var handler = new TransientEventHandler<EventIn>(handlerCreator);
            RegisterEventHandler(handler);
            return this;
        }

        IOutboxConsumer IEventOutboxConsumer.ForwardWithInterception<Event, Interceptor, Module>()
        {
            Func<Event, Event> mapping = eventData => eventData;
            Func<IEventHandler<Event>> handlerCreator = () =>
            {
                var interceptor = _instanceCreator.CreateInstance<Interceptor>();
                return _instanceCreator.CreateInstance<EventDownForwarder<Event, Event, Module>>(mapping, interceptor);
            };
            var handler = new TransientEventHandler<Event>(handlerCreator);
            RegisterEventHandler(handler);
            return this;
        }

        IOutboxConsumer IEventOutboxConsumer.ForwardWithInterception<EventIn, EventOut, Interceptor, Module>(Func<EventIn, EventOut> eventMapping)
        {
            Func<IEventHandler<EventIn>> handlerCreator = () =>
            {
                var interceptor = _instanceCreator.CreateInstance<Interceptor>();
                return _instanceCreator.CreateInstance<EventDownForwarder<EventIn, EventOut, Module>>(eventMapping, interceptor);
            };
            var handler = new TransientEventHandler<EventIn>(handlerCreator);
            RegisterEventHandler(handler);
            return this;
        }

        IOutboxConsumer IEventOutboxConsumer.Expose<Event>()
        {
            Func<Event, Event> mapping = eventData => eventData;
            Func<IEventHandler<Event>> handlerCreator = () =>
            {
                return _instanceCreator.CreateInstance<EventUpForwarder<Event, Event>>(mapping);
            };
            var handler = new TransientEventHandler<Event>(handlerCreator);
            RegisterEventHandler(handler);
            return this;
        }

        IOutboxConsumer IEventOutboxConsumer.Expose<EventIn, EventOut>(Func<EventIn, EventOut> eventMapping)
        {
            Func<IEventHandler<EventIn>> handlerCreator = () =>
            {
                return _instanceCreator.CreateInstance<EventUpForwarder<EventIn, EventOut>>(eventMapping);
            };
            var handler = new TransientEventHandler<EventIn>(handlerCreator);
            RegisterEventHandler(handler);
            return this;
        }

        IOutboxConsumer IEventOutboxConsumer.ExposeWithInterception<Event, Interceptor>()
        {
            Func<Event, Event> mapping = eventData => eventData;
            Func<IEventHandler<Event>> handlerCreator = () =>
            {
                var interceptor = _instanceCreator.CreateInstance<Interceptor>();
                return _instanceCreator.CreateInstance<EventUpForwarder<Event, Event>>(mapping, interceptor);
            };
            var handler = new TransientEventHandler<Event>(handlerCreator);
            RegisterEventHandler(handler);
            return this;
        }

        IOutboxConsumer IEventOutboxConsumer.ExposeWithInterception<EventIn, EventOut, Interceptor>(Func<EventIn, EventOut> eventMapping)
        {
            Func<IEventHandler<EventIn>> handlerCreator = () =>
            {
                var interceptor = _instanceCreator.CreateInstance<Interceptor>();
                return _instanceCreator.CreateInstance<EventUpForwarder<EventIn, EventOut>>(eventMapping, interceptor);
            };
            var handler = new TransientEventHandler<EventIn>(handlerCreator);
            RegisterEventHandler(handler);
            return this;
        }

        private void RegisterEventHandler<Event>(IEventHandler<Event> handler)
            where Event : class, IEvent
        {
            ValidateIfMessageIsDefined(typeof(Event), nameof(Event));
            _moduleInbox.RegisterEventHandler(handler);
            MarkMessageAsHandled(typeof(Event));
        }
    }
}
