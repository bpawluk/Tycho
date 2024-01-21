using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Forwarders;
using Tycho.Messaging.Handlers;
using Tycho.Messaging.Interceptors;
using Tycho.Messaging.Payload;

namespace Tycho.Contract.Inbox.Builder
{
    internal partial class InboxBuilder : IInboxDefinition
    {
        public IInboxDefinition SubscribesTo<Event>(Action<Event> action)
            where Event : class, IEvent
        {
            var handler = new LambdaWrappingEventHandler<Event>(action);
            _moduleInbox.RegisterEventHandler(handler);
            return this;
        }

        public IInboxDefinition SubscribesTo<Event>(Func<Event, Task> function)
            where Event : class, IEvent
        {
            var handler = new LambdaWrappingEventHandler<Event>(function);
            _moduleInbox.RegisterEventHandler(handler);
            return this;
        }

        public IInboxDefinition SubscribesTo<Event>(Func<Event, CancellationToken, Task> function)
            where Event : class, IEvent
        {
            var handler = new LambdaWrappingEventHandler<Event>(function);
            _moduleInbox.RegisterEventHandler(handler);
            return this;
        }

        public IInboxDefinition SubscribesTo<Event>(IEventHandler<Event> handler)
            where Event : class, IEvent
        {
            _moduleInbox.RegisterEventHandler(handler);
            return this;
        }

        public IInboxDefinition SubscribesTo<Event, Handler>()
            where Handler : class, IEventHandler<Event>
            where Event : class, IEvent
        {
            Func<Handler> handlerCreator = () => _instanceCreator.CreateInstance<Handler>();
            var handler = new TransientEventHandler<Event>(handlerCreator);
            _moduleInbox.RegisterEventHandler(handler);
            return this;
        }

        public IInboxDefinition ForwardsEvent<Event, Module>()
            where Event : class, IEvent
            where Module : TychoModule
        {
            Func<Event, Event> mapping = eventData => eventData;
            Func<EventDownForwarder<Event, Event, Module>> forwarderCreator = () =>
            {
                return _instanceCreator.CreateInstance<EventDownForwarder<Event, Event, Module>>(mapping);
            };
            var handler = new TransientEventHandler<Event>(forwarderCreator);
            _moduleInbox.RegisterEventHandler(handler);
            return this;
        }

        public IInboxDefinition ForwardsEvent<Event, Interceptor, Module>()
            where Event : class, IEvent
            where Interceptor : class, IEventInterceptor<Event>
            where Module : TychoModule
        {
            Func<Event, Event> mapping = eventData => eventData;
            Func<EventDownForwarder<Event, Event, Module>> forwarderCreator = () =>
            {
                var interceptor = _instanceCreator.CreateInstance<Interceptor>();
                return _instanceCreator.CreateInstance<EventDownForwarder<Event, Event, Module>>(mapping, interceptor);
            };
            var handler = new TransientEventHandler<Event>(forwarderCreator);
            _moduleInbox.RegisterEventHandler(handler);
            return this;
        }

        public IInboxDefinition ForwardsEvent<EventIn, EventOut, Module>(Func<EventIn, EventOut> eventMapping)
            where EventIn : class, IEvent
            where EventOut : class, IEvent
            where Module : TychoModule
        {
            Func<EventDownForwarder<EventIn, EventOut, Module>> forwarderCreator = () =>
            {
                return _instanceCreator.CreateInstance<EventDownForwarder<EventIn, EventOut, Module>>(eventMapping);
            };
            var handler = new TransientEventHandler<EventIn>(forwarderCreator);
            _moduleInbox.RegisterEventHandler(handler);
            return this;
        }

        public IInboxDefinition ForwardsEvent<EventIn, EventOut, Interceptor, Module>(Func<EventIn, EventOut> eventMapping)
            where EventIn : class, IEvent
            where EventOut : class, IEvent
            where Interceptor : class, IEventInterceptor<EventOut>
            where Module : TychoModule
        {
            Func<EventDownForwarder<EventIn, EventOut, Module>> forwarderCreator = () =>
            {
                var interceptor = _instanceCreator.CreateInstance<Interceptor>();
                return _instanceCreator.CreateInstance<EventDownForwarder<EventIn, EventOut, Module>>(eventMapping, interceptor);
            };
            var handler = new TransientEventHandler<EventIn>(forwarderCreator);
            _moduleInbox.RegisterEventHandler(handler);
            return this;
        }
    }
}
