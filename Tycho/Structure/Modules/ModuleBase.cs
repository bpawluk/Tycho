using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging;
using Tycho.Messaging.Payload;

namespace Tycho.Structure.Modules
{
    internal class ModuleBase : IModule
    {
        protected IMessageBroker? _messageBroker;

        public void Publish<Event>(Event eventData, CancellationToken cancellationToken)
            where Event : class, IEvent
        {
            if (_messageBroker is null)
            {
                throw new InvalidOperationException("Could not publish an event " +
                    "because this module does have a message broker defined");
            }
            _messageBroker.Publish(eventData, cancellationToken);
        }

        public Task Execute<Request>(Request requestData, CancellationToken cancellationToken)
            where Request : class, IRequest
        {
            if (_messageBroker is null)
            {
                throw new InvalidOperationException("Could not execute a request " +
                    "because this module does have a message broker defined");
            }
            return _messageBroker.Execute(requestData, cancellationToken);
        }

        public Task<Response> Execute<Request, Response>(Request requestData, CancellationToken cancellationToken)
            where Request : class, IRequest<Response>
        {
            if (_messageBroker is null)
            {
                throw new InvalidOperationException("Could not execute a request " +
                    "because this module does have a message broker defined");
            }
            return _messageBroker.Execute<Request, Response>(requestData, cancellationToken);
        }

        public void SetMessageBroker(IMessageBroker broker)
        {
            if (_messageBroker is null)
            {
                _messageBroker = broker;
            }
            else
            {
                throw new InvalidOperationException("Message broker for this module is already defined");
            }
        }
    }
}
