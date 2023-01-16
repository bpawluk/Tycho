using System;
using System.Collections.Concurrent;
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
        private readonly ConcurrentDictionary<Type, bool> _messageRegistry;

        public OutboxBuilder(IInstanceCreator instanceCreator, IMessageRouter moduleInbox)
        {
            _instanceCreator = instanceCreator;
            _moduleInbox = moduleInbox;
            _messageRegistry = new ConcurrentDictionary<Type, bool>();
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

        public IOutboxConsumer HandleQuery<Query, Response>(Response response)
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

        private void MarkMessageAsHandled(Type type) => _messageRegistry.TryUpdate(type, true, false);

        private bool ShouldMessageBeHandled(Type type) => !typeof(IEvent).IsAssignableFrom(type);
    }
}
