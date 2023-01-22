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

        public Task Execute<Command>(Command commandData, CancellationToken cancellationToken)
            where Command : class, ICommand
        {
            if (commandData is null)
            {
                throw new ArgumentException($"{nameof(commandData)} cannot be null", nameof(commandData));
            }

            var commandHandler = _messageRouter.GetCommandHandler<Command>();
            return commandHandler.Handle(commandData, cancellationToken);
        }

        public Task<Response> Execute<Query, Response>(Query queryData, CancellationToken cancellationToken)
            where Query : class, IQuery<Response>
        {
            if (queryData is null)
            {
                throw new ArgumentException($"{nameof(queryData)} cannot be null", nameof(queryData));
            }

            var queryHandler = _messageRouter.GetQueryHandler<Query, Response>();
            return queryHandler.Handle(queryData, cancellationToken);
        }
    }
}
