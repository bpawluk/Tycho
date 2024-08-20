using System;
using System.Collections.Generic;
using System.Linq;
using Tycho.Messaging.Handlers;
using Tycho.Messaging.Payload;

namespace Tycho.Messaging
{
    internal class MessageRouter : IMessageRouter
    {
        private readonly IDictionary<Type, List<IEventHandler>> _eventHandlers;
        private readonly IDictionary<Type, ICommandHandler> _commandHandlers;
        private readonly IDictionary<Type, IQueryHandler> _queryHandlers;

        public MessageRouter()
        {
            _eventHandlers = new Dictionary<Type, List<IEventHandler>>();
            _commandHandlers = new Dictionary<Type, ICommandHandler>();
            _queryHandlers = new Dictionary<Type, IQueryHandler>();
        }

        public IEnumerable<IEventHandler<Event>> GetEventHandlers<Event>()
            where Event : class, IEvent
        {
            if (_eventHandlers.TryGetValue(typeof(Event), out var eventHandlers))
            {
                return eventHandlers.Select(handler => (IEventHandler<Event>)handler);
            }

            return Enumerable.Empty<IEventHandler<Event>>();
        }

        public ICommandHandler<Command> GetCommandHandler<Command>()
            where Command : class, IRequest
        {
            if (_commandHandlers.TryGetValue(typeof(Command), out var commandHandler))
            {
                return (ICommandHandler<Command>)commandHandler;
            }

            throw new KeyNotFoundException($"Command handler for {typeof(Command).Name} was not registered");
        }

        public IQueryHandler<Query, Response> GetQueryHandler<Query, Response>()
            where Query : class, IRequest<Response>
        {
            if (_queryHandlers.TryGetValue(typeof(Query), out var queryHandler))
            {
                return (IQueryHandler<Query, Response>)queryHandler;
            }

            throw new KeyNotFoundException($"Query handler for {typeof(Query).Name} was not registered");
        }

        public void RegisterEventHandler<Event>(IEventHandler<Event> eventHandler)
            where Event : class, IEvent
        {
            if (eventHandler is null)
            {
                throw new ArgumentException($"{nameof(eventHandler)} cannot be null", nameof(eventHandler));
            }

            if (!_eventHandlers.ContainsKey(typeof(Event)))
            {
                _eventHandlers[typeof(Event)] = new List<IEventHandler>();
            }

            _eventHandlers[typeof(Event)].Add(eventHandler);
        }

        public void RegisterCommandHandler<Command>(ICommandHandler<Command> commandHandler)
            where Command : class, IRequest
        {
            if (commandHandler is null)
            {
                throw new ArgumentException($"{nameof(commandHandler)} cannot be null", nameof(commandHandler));
            }

            if (!_commandHandlers.TryAdd(typeof(Command), commandHandler))
            {
                throw new ArgumentException($"Command handler for {typeof(Command).Name} already registered", nameof(commandHandler));
            }
        }

        public void RegisterQueryHandler<Query, Response>(IQueryHandler<Query, Response> queryHandler)
            where Query : class, IRequest<Response>
        {
            if (queryHandler is null)
            {
                throw new ArgumentException($"{nameof(queryHandler)} cannot be null", nameof(queryHandler));
            }

            if (!_queryHandlers.TryAdd(typeof(Query), queryHandler))
            {
                throw new ArgumentException($"Query handler for {typeof(Query).Name} already registered", nameof(queryHandler));
            }
        }
    }
}
