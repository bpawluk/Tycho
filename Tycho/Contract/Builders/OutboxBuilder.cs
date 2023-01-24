using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tycho.DependencyInjection;
using Tycho.Messaging;
using Tycho.Messaging.Handlers;
using Tycho.Messaging.Payload;

namespace Tycho.Contract.Builders
{
    internal class OutboxBuilder : IOutboxDefinition, IOutboxConsumer
    {
        private readonly IInstanceCreator _instanceCreator;
        private readonly IMessageRouter _moduleInbox;
        private readonly IDictionary<Type, bool> _messageRegistry;

        public OutboxBuilder(IInstanceCreator instanceCreator, IMessageRouter moduleInbox)
        {
            _instanceCreator = instanceCreator;
            _moduleInbox = moduleInbox;
            _messageRegistry = new Dictionary<Type, bool>();
        }

        #region IOutboxDefiner
        public IOutboxDefinition Publishes<Event>() where Event
            : class, IEvent
        {
            AddMessageDefinition(typeof(Event), nameof(Event));
            return this;
        }

        public IOutboxDefinition Sends<Command>() where Command
            : class, ICommand
        {
            AddMessageDefinition(typeof(Command), nameof(Command));
            return this;
        }

        public IOutboxDefinition Sends<Query, Response>()
            where Query : class, IQuery<Response>
        {
            AddMessageDefinition(typeof(Query), nameof(Query));
            return this;
        }
        #endregion

        #region IOutboxConsumer.Events
        public IOutboxConsumer PassOn<Event, Module>()
            where Event : class, IEvent
            where Module : TychoModule
        {
            Func<Event, Event> mapping = eventData => eventData;
            Func<IEventHandler<Event>> handlerCreator = () => _instanceCreator
                .CreateInstance<DownForwardingEventHandler<Event, Event, Module>>(mapping);
            var handler = new TransientEventHandler<Event>(handlerCreator);
            RegisterEventHandler(handler);
            return this;
        }

        public IOutboxConsumer PassOn<EventIn, EventOut, Module>(Func<EventIn, EventOut> mapping)
            where EventIn : class, IEvent
            where EventOut : class, IEvent
            where Module : TychoModule
        {
            Func<IEventHandler<EventIn>> handlerCreator = () => _instanceCreator
                .CreateInstance<DownForwardingEventHandler<EventIn, EventOut, Module>>(mapping);
            var handler = new TransientEventHandler<EventIn>(handlerCreator);
            RegisterEventHandler(handler);
            return this;
        }

        public IOutboxConsumer ExposeEvent<Event>()
            where Event : class, IEvent
        {
            Func<Event, Event> mapping = eventData => eventData;
            var handler = _instanceCreator.CreateInstance<UpForwardingEventHandler<Event, Event>>(mapping);
            RegisterEventHandler(handler);
            return this;
        }

        public IOutboxConsumer ExposeEvent<EventIn, EventOut>(Func<EventIn, EventOut> mapping)
            where EventIn : class, IEvent
            where EventOut : class, IEvent
        {
            var handler = _instanceCreator.CreateInstance<UpForwardingEventHandler<EventIn, EventOut>>(mapping);
            RegisterEventHandler(handler);
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

        private void RegisterEventHandler<Event>(IEventHandler<Event> handler)
            where Event : class, IEvent
        {
            ValidateIfMessageIsDefined(typeof(Event), nameof(Event));
            _moduleInbox.RegisterEventHandler(handler);
            MarkMessageAsHandled(typeof(Event));
        }
        #endregion

        #region IOutboxConsumer.Commands
        public IOutboxConsumer Forward<Command, Module>()
            where Command : class, ICommand
            where Module : TychoModule
        {
            Func<Command, Command> mapping = commandData => commandData;
            Func<ICommandHandler<Command>> handlerCreator = () => _instanceCreator
                .CreateInstance<DownForwardingCommandHandler<Command, Command, Module>>(mapping);
            var handler = new TransientCommandHandler<Command>(handlerCreator);
            RegisterCommandHandler(handler);
            return this;
        }

        public IOutboxConsumer Forward<CommandIn, CommandOut, Module>(Func<CommandIn, CommandOut> mapping)
            where CommandIn : class, ICommand
            where CommandOut : class, ICommand
            where Module : TychoModule
        {
            Func<ICommandHandler<CommandIn>> handlerCreator = () => _instanceCreator
                .CreateInstance<DownForwardingCommandHandler<CommandIn, CommandOut, Module>>(mapping);
            var handler = new TransientCommandHandler<CommandIn>(handlerCreator);
            RegisterCommandHandler(handler);
            return this;
        }

        public IOutboxConsumer ExposeCommand<Command>()
            where Command : class, ICommand
        {
            Func<Command, Command> mapping = commandData => commandData;
            var handler = _instanceCreator.CreateInstance<UpForwardingCommandHandler<Command, Command>>(mapping);
            RegisterCommandHandler(handler);
            return this;
        }

        public IOutboxConsumer ExposeCommand<CommandIn, CommandOut>(Func<CommandIn, CommandOut> mapping)
            where CommandIn : class, ICommand
            where CommandOut : class, ICommand
        {
            var handler = _instanceCreator.CreateInstance<UpForwardingCommandHandler<CommandIn, CommandOut>>(mapping);
            RegisterCommandHandler(handler);
            return this;
        }

        public IOutboxConsumer HandleCommand<Command>(Action<Command> action)
            where Command : class, ICommand
        {
            var handler = new LambdaWrappingCommandHandler<Command>(action);
            RegisterCommandHandler(handler);
            return this;
        }

        public IOutboxConsumer HandleCommand<Command>(Func<Command, Task> function)
            where Command : class, ICommand
        {
            var handler = new LambdaWrappingCommandHandler<Command>(function);
            RegisterCommandHandler(handler);
            return this;
        }

        public IOutboxConsumer HandleCommand<Command>(Func<Command, CancellationToken, Task> function)
            where Command : class, ICommand
        {
            var handler = new LambdaWrappingCommandHandler<Command>(function);
            RegisterCommandHandler(handler);
            return this;
        }

        public IOutboxConsumer HandleCommand<Command>(ICommandHandler<Command> handler)
            where Command : class, ICommand
        {
            RegisterCommandHandler(handler);
            return this;
        }

        public IOutboxConsumer HandleCommand<Command, Handler>()
            where Handler : class, ICommandHandler<Command>
            where Command : class, ICommand
        {
            Func<Handler> handlerCreator = () => _instanceCreator.CreateInstance<Handler>();
            var handler = new TransientCommandHandler<Command>(handlerCreator);
            RegisterCommandHandler(handler);
            return this;
        }

        public IOutboxConsumer IgnoreCommand<Command>()
            where Command : class, ICommand
        {
            var handler = new StubCommandHandler<Command>();
            RegisterCommandHandler(handler);
            return this;
        }

        private void RegisterCommandHandler<Command>(ICommandHandler<Command> handler)
            where Command : class, ICommand
        {
            ValidateIfMessageIsDefined(typeof(Command), nameof(Command));
            _moduleInbox.RegisterCommandHandler(handler);
            MarkMessageAsHandled(typeof(Command));
        }
        #endregion

        #region IOutboxConsumer.Queries
        public IOutboxConsumer Forward<Query, Response, Module>()
            where Query : class, IQuery<Response>
            where Module : TychoModule
        {
            Func<Query, Query> mapping = queryData => queryData;
            Func<IQueryHandler<Query, Response>> handlerCreator = () => _instanceCreator
                .CreateInstance<DownForwardingQueryHandler<Query, Query, Response, Module>>(mapping);
            var handler = new TransientQueryHandler<Query, Response>(handlerCreator);
            RegisterQueryHandler(handler);
            return this;
        }

        public IOutboxConsumer Forward<QueryIn, QueryOut, Response, Module>(Func<QueryIn, QueryOut> mapping)
            where QueryIn : class, IQuery<Response>
            where QueryOut : class, IQuery<Response>
            where Module : TychoModule
        {
            Func<IQueryHandler<QueryIn, Response>> handlerCreator = () => _instanceCreator
                .CreateInstance<DownForwardingQueryHandler<QueryIn, QueryOut, Response, Module>>(mapping);
            var handler = new TransientQueryHandler<QueryIn, Response>(handlerCreator);
            RegisterQueryHandler(handler);
            return this;
        }

        public IOutboxConsumer ExposeQuery<Query, Response>()
            where Query : class, IQuery<Response>
        {
            Func<Query, Query> mapping = queryData => queryData;
            var handler = _instanceCreator
                .CreateInstance<UpForwardingQueryHandler<Query, Query, Response>>(mapping);
            RegisterQueryHandler(handler);
            return this;
        }

        public IOutboxConsumer ExposeQuery<QueryIn, QueryOut, Response>(Func<QueryIn, QueryOut> mapping)
            where QueryIn : class, IQuery<Response>
            where QueryOut : class, IQuery<Response>
        {
            var handler = _instanceCreator
                .CreateInstance<UpForwardingQueryHandler<QueryIn, QueryOut, Response>>(mapping);
            RegisterQueryHandler(handler);
            return this;
        }

        public IOutboxConsumer HandleQuery<Query, Response>(Func<Query, Response> function)
            where Query : class, IQuery<Response>
        {
            var handler = new LambdaWrappingQueryHandler<Query, Response>(function);
            RegisterQueryHandler(handler);
            return this;
        }

        public IOutboxConsumer HandleQuery<Query, Response>(Func<Query, Task<Response>> function)
            where Query : class, IQuery<Response>
        {
            var handler = new LambdaWrappingQueryHandler<Query, Response>(function);
            RegisterQueryHandler(handler);
            return this;
        }

        public IOutboxConsumer HandleQuery<Query, Response>(Func<Query, CancellationToken, Task<Response>> function)
            where Query : class, IQuery<Response>
        {
            var handler = new LambdaWrappingQueryHandler<Query, Response>(function);
            RegisterQueryHandler(handler);
            return this;
        }

        public IOutboxConsumer HandleQuery<Query, Response>(IQueryHandler<Query, Response> handler)
            where Query : class, IQuery<Response>
        {
            RegisterQueryHandler(handler);
            return this;
        }

        public IOutboxConsumer HandleQuery<Query, Response, Handler>()
            where Handler : class, IQueryHandler<Query, Response>
            where Query : class, IQuery<Response>
        {
            Func<Handler> handlerCreator = () => _instanceCreator.CreateInstance<Handler>();
            var handler = new TransientQueryHandler<Query, Response>(handlerCreator);
            RegisterQueryHandler(handler);
            return this;
        }

        public IOutboxConsumer IgnoreQuery<Query, Response>(Response response)
            where Query : class, IQuery<Response>
        {
            var handler = new StubQueryHandler<Query, Response>(response);
            RegisterQueryHandler(handler);
            return this;
        }

        public IOutboxConsumer RegisterQueryHandler<Query, Response>(IQueryHandler<Query, Response> handler)
            where Query : class, IQuery<Response>
        {
            ValidateIfMessageIsDefined(typeof(Query), nameof(Query));
            _moduleInbox.RegisterQueryHandler(handler);
            MarkMessageAsHandled(typeof(Query));
            return this;
        }
        #endregion

        public IMessageBroker Build()
        {
            ValidateIfAllMethodsAreHandled();
            return new MessageBroker(_moduleInbox);
        }

        private void AddMessageDefinition(Type messageType, string messageKind)
        {
            if (!_messageRegistry.TryAdd(messageType, false))
            {
                throw new ArgumentException(
                    $"The {messageType.Name} {messageKind} is already defined for this module",
                    nameof(messageType));
            }
        }

        private void ValidateIfMessageIsDefined(Type messageType, string messageKind)
        {
            if (!_messageRegistry.ContainsKey(messageType))
            {
                throw new InvalidOperationException(
                    $"Could not register a message handler because " +
                    $"the {messageType.Name} {messageKind} is not defined for this module");
            }
        }

        private void ValidateIfAllMethodsAreHandled()
        {
            var missingHandlers = _messageRegistry.Where(entry => !entry.Value && ShouldMessageBeHandled(entry.Key));
            if (missingHandlers.Any())
            {
                throw new InvalidOperationException(
                    $"Could not construct a message broker for this module " +
                    $"because one or more messages are missing required handlers. " +
                    $"Please provide handler for: {string.Join(',', missingHandlers.Select(entry => entry.Key))}");
            }
        }

        private void MarkMessageAsHandled(Type type) => _messageRegistry[type] = true;

        private bool ShouldMessageBeHandled(Type type) => !typeof(IEvent).IsAssignableFrom(type);
    }
}
