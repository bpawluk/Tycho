using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Forwarders;
using Tycho.Messaging.Handlers;

namespace Tycho.Contract.Inbox.Builder
{
    internal partial class InboxBuilder : IInboxDefinition, IRequestInboxDefinition, IEventInboxDefinition
    {
        IInboxDefinition IEventInboxDefinition.Handle<Event>(Action<Event> action)
        {
            var handler = new LambdaWrappingEventHandler<Event>(action);
            _moduleInbox.RegisterEventHandler(handler);
            return this;
        }

        IInboxDefinition IEventInboxDefinition.Handle<Event>(Func<Event, Task> function)
        {
            var handler = new LambdaWrappingEventHandler<Event>(function);
            _moduleInbox.RegisterEventHandler(handler);
            return this;
        }

        IInboxDefinition IEventInboxDefinition.Handle<Event>(Func<Event, CancellationToken, Task> function)
        {
            var handler = new LambdaWrappingEventHandler<Event>(function);
            _moduleInbox.RegisterEventHandler(handler);
            return this;
        }

        IInboxDefinition IEventInboxDefinition.Handle<Event>(IEventHandler<Event> handler)
        {
            _moduleInbox.RegisterEventHandler(handler);
            return this;
        }

        IInboxDefinition IEventInboxDefinition.Handle<Event, Handler>()
        {
            Func<Handler> handlerCreator = () => _instanceCreator.CreateInstance<Handler>();
            var handler = new TransientEventHandler<Event>(handlerCreator);
            _moduleInbox.RegisterEventHandler(handler);
            return this;
        }

        IInboxDefinition IEventInboxDefinition.Forward<Event, Module>()
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

        IInboxDefinition IEventInboxDefinition.ForwardWithInterception<Event, Interceptor, Module>()
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

        IInboxDefinition IEventInboxDefinition.Forward<EventIn, EventOut, Module>(Func<EventIn, EventOut> eventMapping)
        {
            Func<EventDownForwarder<EventIn, EventOut, Module>> forwarderCreator = () =>
            {
                return _instanceCreator.CreateInstance<EventDownForwarder<EventIn, EventOut, Module>>(eventMapping);
            };
            var handler = new TransientEventHandler<EventIn>(forwarderCreator);
            _moduleInbox.RegisterEventHandler(handler);
            return this;
        }

        IInboxDefinition IEventInboxDefinition.ForwardWithInterception<EventIn, EventOut, Interceptor, Module>(Func<EventIn, EventOut> eventMapping)
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
