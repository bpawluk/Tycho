using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Payload;

namespace Tycho.Messaging.Handlers
{
    internal abstract class TransientBase<Handler>
    {
        protected readonly Func<Handler> _createHandler;

        public TransientBase(Func<Handler> handlerCreator)
        {
            _createHandler = handlerCreator;
        }
    }

    internal class TransientEventHandler<Event>
        : TransientBase<IEventHandler<Event>>
        , IEventHandler<Event>
        where Event : class, IEvent
    {
        public TransientEventHandler(Func<IEventHandler<Event>> handlerCreator)
            : base(handlerCreator) { }

        public Task Handle(Event eventData, CancellationToken cancellationToken)
        {
            var handler = _createHandler();
            return handler.Handle(eventData, cancellationToken);
        }
    }

    internal class TransientRequestHandler<Request>
        : TransientBase<IRequestHandler<Request>>
        , IRequestHandler<Request>
        where Request : class, IRequest
    {
        public TransientRequestHandler(Func<IRequestHandler<Request>> handlerCreator)
            : base(handlerCreator) { }

        public Task Handle(Request requestData, CancellationToken cancellationToken)
        {
            var handler = _createHandler();
            return handler.Handle(requestData, cancellationToken);
        }
    }

    internal class TransientRequestHandler<Request, Response>
        : TransientBase<IRequestHandler<Request, Response>>
        , IRequestHandler<Request, Response>
        where Request : class, IRequest<Response>
    {
        public TransientRequestHandler(Func<IRequestHandler<Request, Response>> handlerCreator)
            : base(handlerCreator) { }

        public Task<Response> Handle(Request requestData, CancellationToken cancellationToken)
        {
            var handler = _createHandler();
            return handler.Handle(requestData, cancellationToken);
        }
    }
}
