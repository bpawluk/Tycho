using NewTycho.Events;
using NewTycho.Requests;
using System;
using System.Collections.Generic;

namespace NewTycho.Messaging
{
    internal class HandlerRegistry
    {
        private readonly string _id;
        private readonly Dictionary<Type, IHandlerRegistration> _registry;

        public HandlerRegistry(string id)
        {
            _id = id;
            _registry = new Dictionary<Type, IHandlerRegistration>();
        }

        public void RegisterEventHandler<TEvent>()
            where TEvent : IEvent
        {
            _registry[typeof(TEvent)] = registration;
        }

        public void RegisterRequestHandler<TRequest>()
            where TRequest : IRequest
        {
            _registry[typeof(TRequest)] = registration;
        }

        public void RegisterRequestHandler<TRequest, TResponse>()
            where TRequest : IRequest<TResponse>
        {
            _registry[typeof(TRequest)] = registration;
        }
    }
}
