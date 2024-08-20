using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Payload;

namespace Tycho.Messaging
{
    internal class MessageBroker : IMessageBroker
    {
        private readonly IMessageRouter _messageRouter;

        public MessageBroker(IMessageRouter messageRouter)
        {
            _messageRouter = messageRouter;
        }

        public void Publish<Event>(Event eventData, CancellationToken cancellationToken)
            where Event : class, IEvent
        {
            if (eventData is null)
            {
                throw new ArgumentException($"{nameof(eventData)} cannot be null", nameof(eventData));
            }

            foreach (var eventHandler in _messageRouter.GetEventHandlers<Event>())
            {
                _ = eventHandler.Handle(eventData, cancellationToken);
            }
        }

        public Task Execute<Request>(Request requestData, CancellationToken cancellationToken)
            where Request : class, IRequest
        {
            if (requestData is null)
            {
                throw new ArgumentException($"{nameof(requestData)} cannot be null", nameof(requestData));
            }

            var requestHandler = _messageRouter.GetRequestHandler<Request>();
            return requestHandler.Handle(requestData, cancellationToken);
        }

        public Task<Response> Execute<Request, Response>(Request requestData, CancellationToken cancellationToken)
            where Request : class, IRequest<Response>
        {
            if (requestData is null)
            {
                throw new ArgumentException($"{nameof(requestData)} cannot be null", nameof(requestData));
            }

            var requestHandler = _messageRouter.GetRequestWithResponseHandler<Request, Response>();
            return requestHandler.Handle(requestData, cancellationToken);
        }
    }
}
