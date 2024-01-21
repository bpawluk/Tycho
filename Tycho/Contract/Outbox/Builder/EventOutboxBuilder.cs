using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Forwarders;
using Tycho.Messaging.Handlers;
using Tycho.Messaging.Interceptors;
using Tycho.Messaging.Payload;

namespace Tycho.Contract.Outbox.Builder
{
    internal partial class OutboxBuilder : IOutboxDefinition, IOutboxConsumer
    {
        public IOutboxDefinition Publishes<Event>() where Event
            : class, IEvent
        {
            AddMessageDefinition(typeof(Event), nameof(Event));
            return this;
        }

        public IOutboxConsumer HandleEvent<Event>(Action<Event> action)
            where Event : class, IEvent
        {
            var handler = new LambdaWrappingEventHandler<Event>(action);
            RegisterEventHandler(handler);
            return this;
        }

        public IOutboxConsumer HandleEvent<Event>(Func<Event, Task> function)
            where Event : class, IEvent
        {
            var handler = new LambdaWrappingEventHandler<Event>(function);
            RegisterEventHandler(handler);
            return this;
        }

        public IOutboxConsumer HandleEvent<Event>(Func<Event, CancellationToken, Task> function)
            where Event : class, IEvent
        {
            var handler = new LambdaWrappingEventHandler<Event>(function);
            RegisterEventHandler(handler);
            return this;
        }

        public IOutboxConsumer HandleEvent<Event>(IEventHandler<Event> handler)
            where Event : class, IEvent
        {
            RegisterEventHandler(handler);
            return this;
        }

        public IOutboxConsumer HandleEvent<Event, Handler>()
            where Handler : class, IEventHandler<Event>
            where Event : class, IEvent
        {
            Func<Handler> handlerCreator = () => _instanceCreator.CreateInstance<Handler>();
            var handler = new TransientEventHandler<Event>(handlerCreator);
            RegisterEventHandler(handler);
            return this;
        }

        public IOutboxConsumer ForwardEvent<Event, Module>()
            where Event : class, IEvent
            where Module : TychoModule
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

        public IOutboxConsumer ForwardEvent<Event, Interceptor, Module>()
            where Event : class, IEvent
            where Interceptor : class, IEventInterceptor<Event>
            where Module : TychoModule
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

        public IOutboxConsumer ForwardEvent<EventIn, EventOut, Module>(Func<EventIn, EventOut> eventMapping)
            where EventIn : class, IEvent
            where EventOut : class, IEvent
            where Module : TychoModule
        {
            Func<IEventHandler<EventIn>> handlerCreator = () =>
            {
                return _instanceCreator.CreateInstance<EventDownForwarder<EventIn, EventOut, Module>>(eventMapping);
            };
            var handler = new TransientEventHandler<EventIn>(handlerCreator);
            RegisterEventHandler(handler);
            return this;
        }

        public IOutboxConsumer ForwardEvent<EventIn, EventOut, Interceptor, Module>(Func<EventIn, EventOut> eventMapping)
            where EventIn : class, IEvent
            where EventOut : class, IEvent
            where Interceptor : class, IEventInterceptor<EventOut>
            where Module : TychoModule
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

        public IOutboxConsumer ExposeEvent<Event>()
            where Event : class, IEvent
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

        public IOutboxConsumer ExposeEvent<Event, Interceptor>()
            where Event : class, IEvent
            where Interceptor : class, IEventInterceptor<Event>
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

        public IOutboxConsumer ExposeEvent<EventIn, EventOut>(Func<EventIn, EventOut> eventMapping)
            where EventIn : class, IEvent
            where EventOut : class, IEvent
        {
            Func<IEventHandler<EventIn>> handlerCreator = () =>
            {
                return _instanceCreator.CreateInstance<EventUpForwarder<EventIn, EventOut>>(eventMapping);
            };
            var handler = new TransientEventHandler<EventIn>(handlerCreator);
            RegisterEventHandler(handler);
            return this;
        }

        public IOutboxConsumer ExposeEvent<EventIn, EventOut, Interceptor>(Func<EventIn, EventOut> eventMapping)
            where EventIn : class, IEvent
            where EventOut : class, IEvent
            where Interceptor : class, IEventInterceptor<EventOut>
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
