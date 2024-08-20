using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Payload;

namespace Tycho.Messaging.Handlers
{
    internal class LambdaWrappingBase<Message, Result>
    {
        protected readonly Func<Message, CancellationToken, Result> _handle;

        public LambdaWrappingBase(Func<Message, CancellationToken, Result> handler)
        {
            _handle = handler;
        }

        protected static Func<Message, CancellationToken, Result> Wrap(Action<Message> handler, Result result)
        {
            return (message, _) =>
            {
                handler(message);
                return result;
            };
        }

        protected static Func<Message, CancellationToken, Result> Wrap(Func<Message, Result> handler)
        {
            return (message, _) => handler(message);
        }
    }

    internal class LambdaWrappingEventHandler<Event>
        : LambdaWrappingBase<Event, Task>
        , IEventHandler<Event>
        where Event : class, IEvent
    {
        public LambdaWrappingEventHandler(Action<Event> handler)
            : base(Wrap(handler, Task.CompletedTask)) { }

        public LambdaWrappingEventHandler(Func<Event, Task> handler)
            : base(Wrap(handler)) { }

        public LambdaWrappingEventHandler(Func<Event, CancellationToken, Task> handler)
            : base(handler) { }

        public Task Handle(Event eventData, CancellationToken cancellationToken)
        {
            return _handle(eventData, cancellationToken);
        }
    }

    internal class LambdaWrappingRequestHandler<Request>
        : LambdaWrappingBase<Request, Task>
        , IRequestHandler<Request>
        where Request : class, IRequest
    {
        public LambdaWrappingRequestHandler(Action<Request> handler)
            : base(Wrap(handler, Task.CompletedTask)) { }

        public LambdaWrappingRequestHandler(Func<Request, Task> handler)
            : base(Wrap(handler)) { }

        public LambdaWrappingRequestHandler(Func<Request, CancellationToken, Task> handler)
            : base(handler) { }

        public Task Handle(Request requestData, CancellationToken cancellationToken)
        {
            return _handle(requestData, cancellationToken);
        }
    }

    internal class LambdaWrappingRequestHandler<Request, Response>
        : LambdaWrappingBase<Request, Task<Response>>
        , IRequestHandler<Request, Response>
        where Request : class, IRequest<Response>
    {
        public LambdaWrappingRequestHandler(Func<Request, Response> handler)
            : base(Wrap(handler)) { }

        public LambdaWrappingRequestHandler(Func<Request, Task<Response>> handler)
            : base(Wrap(handler)) { }

        public LambdaWrappingRequestHandler(Func<Request, CancellationToken, Task<Response>> handler)
            : base(handler) { }

        public Task<Response> Handle(Request requestData, CancellationToken cancellationToken)
        {
            return _handle(requestData, cancellationToken);
        }

        private static Func<Request, CancellationToken, Task<Response>> Wrap(Func<Request, Response> handler)
        {
            return (message, _) => Task.FromResult(handler(message));
        }
    }
}
