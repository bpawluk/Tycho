﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.DependencyInjection;
using Tycho.Messaging.Handlers;
using Tycho.Messaging.Payload;

namespace Tycho.Messaging.Contracts
{
    internal class InboxBuilder : IInboxBuilder
    {
        private readonly IInstanceCreator _instanceCreator;
        private readonly IMessageRouter _moduleInbox;

        public InboxBuilder(IInstanceCreator instanceCreator, IMessageRouter moduleInbox)
        {
            _instanceCreator = instanceCreator;
            _moduleInbox = moduleInbox;
        }

        #region IInboxDefiner.Events
        public IInboxBuilder SubscribesTo<Event>(Action<Event> action)
            where Event : class, IEvent
        {
            var handler = new LambdaWrappingEventHandler<Event>(action);
            _moduleInbox.RegisterEventHandler(handler);
            return this;
        }

        public IInboxBuilder SubscribesTo<Event>(Func<Event, Task> function)
            where Event : class, IEvent
        {
            var handler = new LambdaWrappingEventHandler<Event>(function);
            _moduleInbox.RegisterEventHandler(handler);
            return this;
        }

        public IInboxBuilder SubscribesTo<Event>(Func<Event, CancellationToken, Task> function)
            where Event : class, IEvent
        {
            var handler = new LambdaWrappingEventHandler<Event>(function);
            _moduleInbox.RegisterEventHandler(handler);
            return this;
        }

        public IInboxBuilder SubscribesTo<Event>(IEventHandler<Event> handler)
            where Event : class, IEvent
        {
            _moduleInbox.RegisterEventHandler(handler);
            return this;
        }

        public IInboxBuilder SubscribesTo<Event, Handler>()
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
        public IInboxBuilder Executes<Command>(Action<Command> action)
            where Command : class, ICommand
        {
            var handler = new LambdaWrappingCommandHandler<Command>(action);
            _moduleInbox.RegisterCommandHandler(handler);
            return this;
        }

        public IInboxBuilder Executes<Command>(Func<Command, Task> function)
            where Command : class, ICommand
        {
            var handler = new LambdaWrappingCommandHandler<Command>(function);
            _moduleInbox.RegisterCommandHandler(handler);
            return this;
        }

        public IInboxBuilder Executes<Command>(Func<Command, CancellationToken, Task> function)
            where Command : class, ICommand
        {
            var handler = new LambdaWrappingCommandHandler<Command>(function);
            _moduleInbox.RegisterCommandHandler(handler);
            return this;
        }

        public IInboxBuilder Executes<Command>(ICommandHandler<Command> handler)
            where Command : class, ICommand
        {
            _moduleInbox.RegisterCommandHandler(handler);
            return this;
        }

        public IInboxBuilder Executes<Command, Handler>()
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
        public IInboxBuilder RespondsTo<Query, Response>(Func<Query, Response> function)
            where Query : class, IQuery<Response>
        {
            var handler = new LambdaWrappingQueryHandler<Query, Response>(function);
            _moduleInbox.RegisterQueryHandler(handler);
            return this;
        }

        public IInboxBuilder RespondsTo<Query, Response>(Func<Query, Task<Response>> function)
            where Query : class, IQuery<Response>
        {
            var handler = new LambdaWrappingQueryHandler<Query, Response>(function);
            _moduleInbox.RegisterQueryHandler(handler);
            return this;
        }

        public IInboxBuilder RespondsTo<Query, Response>(Func<Query, CancellationToken, Task<Response>> function)
            where Query : class, IQuery<Response>
        {
            var handler = new LambdaWrappingQueryHandler<Query, Response>(function);
            _moduleInbox.RegisterQueryHandler(handler);
            return this;
        }

        public IInboxBuilder RespondsTo<Query, Response>(IQueryHandler<Query, Response> handler)
            where Query : class, IQuery<Response>
        {
            _moduleInbox.RegisterQueryHandler(handler);
            return this;
        }

        public IInboxBuilder RespondsTo<Query, Response, Handler>()
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
