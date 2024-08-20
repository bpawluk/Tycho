using System;
using System.Collections.Generic;
using System.Linq;
using Tycho.Messaging.Handlers;
using Tycho.Messaging.Payload;

namespace Tycho.Messaging
{
    internal class MessageRouter : IMessageRouter
    {
        private readonly IDictionary<Type, List<IMessageHandler>> _eventHandlers;
        private readonly IDictionary<Type, IMessageHandler> _requestHandlers;
        private readonly IDictionary<Type, IMessageHandler> _requestWithResponseHandlers;

        public MessageRouter()
        {
            _eventHandlers = new Dictionary<Type, List<IMessageHandler>>();
            _requestHandlers = new Dictionary<Type, IMessageHandler>();
            _requestWithResponseHandlers = new Dictionary<Type, IMessageHandler>();
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

        public IRequestHandler<Request> GetRequestHandler<Request>()
            where Request : class, IRequest
        {
            if (_requestHandlers.TryGetValue(typeof(Request), out var requestHandler))
            {
                return (IRequestHandler<Request>)requestHandler;
            }

            throw new KeyNotFoundException($"Request handler for {typeof(Request).Name} was not registered");
        }

        public IRequestHandler<Request, Response> GetRequestWithResponseHandler<Request, Response>()
            where Request : class, IRequest<Response>
        {
            if (_requestWithResponseHandlers.TryGetValue(typeof(Request), out var requestHandler))
            {
                return (IRequestHandler<Request, Response>)requestHandler;
            }

            throw new KeyNotFoundException($"Request handler for {typeof(Request).Name} was not registered");
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
                _eventHandlers[typeof(Event)] = new List<IMessageHandler>();
            }

            _eventHandlers[typeof(Event)].Add(eventHandler);
        }

        public void RegisterRequestHandler<Request>(IRequestHandler<Request> requestHandler)
            where Request : class, IRequest
        {
            if (requestHandler is null)
            {
                throw new ArgumentException($"{nameof(requestHandler)} cannot be null", nameof(requestHandler));
            }

            if (!_requestHandlers.TryAdd(typeof(Request), requestHandler))
            {
                throw new ArgumentException($"Request handler for {typeof(Request).Name} already registered", nameof(requestHandler));
            }
        }

        public void RegisterRequestWithResponseHandler<Request, Response>(IRequestHandler<Request, Response> requestHandler)
            where Request : class, IRequest<Response>
        {
            if (requestHandler is null)
            {
                throw new ArgumentException($"{nameof(requestHandler)} cannot be null", nameof(requestHandler));
            }

            if (!_requestWithResponseHandlers.TryAdd(typeof(Request), requestHandler))
            {
                throw new ArgumentException($"Request handler for {typeof(Request).Name} already registered", nameof(requestHandler));
            }
        }
    }
}
