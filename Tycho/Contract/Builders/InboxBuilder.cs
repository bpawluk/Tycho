using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.DependencyInjection;
using Tycho.Messaging;
using Tycho.Messaging.Handlers;
using Tycho.Messaging.Payload;

namespace Tycho.Contract.Builders
{
    internal class InboxBuilder : IInboxDefinition
    {
        private readonly IInstanceCreator _instanceCreator;
        private readonly IMessageRouter _moduleInbox;

        public InboxBuilder(IInstanceCreator instanceCreator, IMessageRouter moduleInbox)
        {
            _instanceCreator = instanceCreator;
            _moduleInbox = moduleInbox;
        }

        #region IInboxDefiner.Events
        public IInboxDefinition PassesOn<Event, Module>()
            where Event : class, IEvent
            where Module : TychoModule
        {
            Func<Event, Event> mapping = eventData => eventData;
            var handler = _instanceCreator.CreateInstance<DownForwardingEventHandler<Event, Event, Module>>(mapping);
            _moduleInbox.RegisterEventHandler(handler);
            return this;
        }

        public IInboxDefinition PassesOn<EventIn, EventOut, Module>(Func<EventIn, EventOut> mapping)
            where EventIn : class, IEvent
            where EventOut : class, IEvent
            where Module : TychoModule
        {
            var handler = _instanceCreator.CreateInstance<DownForwardingEventHandler<EventIn, EventOut, Module>>(mapping);
            _moduleInbox.RegisterEventHandler(handler);
            return this;
        }

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
        #endregion

        #region IInboxDefiner.Commands
        public IInboxDefinition Forwards<Command, Module>()
            where Command : class, ICommand
            where Module : TychoModule
        {
            Func<Command, Command> mapping = commandData => commandData;
            var handler = _instanceCreator.CreateInstance<DownForwardingCommandHandler<Command, Command, Module>>(mapping);
            _moduleInbox.RegisterCommandHandler(handler);
            return this;
        }

        public IInboxDefinition Forwards<CommandIn, CommandOut, Module>(Func<CommandIn, CommandOut> mapping)
            where CommandIn : class, ICommand
            where CommandOut : class, ICommand
            where Module : TychoModule
        {
            var handler = _instanceCreator
                .CreateInstance<DownForwardingCommandHandler<CommandIn, CommandOut, Module>>(mapping);
            _moduleInbox.RegisterCommandHandler(handler);
            return this;
        }

        public IInboxDefinition Executes<Command>(Action<Command> action)
            where Command : class, ICommand
        {
            var handler = new LambdaWrappingCommandHandler<Command>(action);
            _moduleInbox.RegisterCommandHandler(handler);
            return this;
        }

        public IInboxDefinition Executes<Command>(Func<Command, Task> function)
            where Command : class, ICommand
        {
            var handler = new LambdaWrappingCommandHandler<Command>(function);
            _moduleInbox.RegisterCommandHandler(handler);
            return this;
        }

        public IInboxDefinition Executes<Command>(Func<Command, CancellationToken, Task> function)
            where Command : class, ICommand
        {
            var handler = new LambdaWrappingCommandHandler<Command>(function);
            _moduleInbox.RegisterCommandHandler(handler);
            return this;
        }

        public IInboxDefinition Executes<Command>(ICommandHandler<Command> handler)
            where Command : class, ICommand
        {
            _moduleInbox.RegisterCommandHandler(handler);
            return this;
        }

        public IInboxDefinition Executes<Command, Handler>()
            where Handler : class, ICommandHandler<Command>
            where Command : class, ICommand
        {
            Func<Handler> handlerCreator = () => _instanceCreator.CreateInstance<Handler>();
            var handler = new TransientCommandHandler<Command>(handlerCreator);
            _moduleInbox.RegisterCommandHandler(handler);
            return this;
        }
        #endregion

        #region IInboxDefiner.Queries
        public IInboxDefinition Forwards<Query, Response, Module>()
            where Query : class, IQuery<Response>
            where Module : TychoModule
        {
            Func<Query, Query> mapping = queryData => queryData;
            var handler = _instanceCreator
                .CreateInstance<DownForwardingQueryHandler<Query, Query, Response, Module>>(mapping);
            _moduleInbox.RegisterQueryHandler(handler);
            return this;
        }

        public IInboxDefinition Forwards<QueryIn, QueryOut, Response, Module>(Func<QueryIn, QueryOut> mapping)
            where QueryIn : class, IQuery<Response>
            where QueryOut : class, IQuery<Response>
            where Module : TychoModule
        {
            var handler = _instanceCreator
                .CreateInstance<DownForwardingQueryHandler<QueryIn, QueryOut, Response, Module>>(mapping);
            _moduleInbox.RegisterQueryHandler(handler);
            return this;
        }

        public IInboxDefinition RespondsTo<Query, Response>(Func<Query, Response> function)
            where Query : class, IQuery<Response>
        {
            var handler = new LambdaWrappingQueryHandler<Query, Response>(function);
            _moduleInbox.RegisterQueryHandler(handler);
            return this;
        }

        public IInboxDefinition RespondsTo<Query, Response>(Func<Query, Task<Response>> function)
            where Query : class, IQuery<Response>
        {
            var handler = new LambdaWrappingQueryHandler<Query, Response>(function);
            _moduleInbox.RegisterQueryHandler(handler);
            return this;
        }

        public IInboxDefinition RespondsTo<Query, Response>(Func<Query, CancellationToken, Task<Response>> function)
            where Query : class, IQuery<Response>
        {
            var handler = new LambdaWrappingQueryHandler<Query, Response>(function);
            _moduleInbox.RegisterQueryHandler(handler);
            return this;
        }

        public IInboxDefinition RespondsTo<Query, Response>(IQueryHandler<Query, Response> handler)
            where Query : class, IQuery<Response>
        {
            _moduleInbox.RegisterQueryHandler(handler);
            return this;
        }

        public IInboxDefinition RespondsTo<Query, Response, Handler>()
            where Handler : class, IQueryHandler<Query, Response>
            where Query : class, IQuery<Response>
        {
            Func<Handler> handlerCreator = () => _instanceCreator.CreateInstance<Handler>();
            var handler = new TransientQueryHandler<Query, Response>(handlerCreator);
            _moduleInbox.RegisterQueryHandler(handler);
            return this;
        }
        #endregion

        public IMessageBroker Build() => new MessageBroker(_moduleInbox);
    }
}
