using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Payload;

namespace Tycho.Messaging.Handlers
{
    internal class StubEventHandler<Event> : IEventHandler<Event>
        where Event : class, IEvent
    {
        public Task Handle(Event eventData, CancellationToken cancellationToken) => Task.CompletedTask;
    }

    internal class StubRequestHandler<Request> : IRequestHandler<Request>
        where Request : class, IRequest
    {
        public Task Handle(Request requestData, CancellationToken cancellationToken) => Task.CompletedTask;
    }

    internal class StubRequestHandler<Request, Response> : IRequestHandler<Request, Response>
        where Request : class, IRequest<Response>
    {
        private readonly Response _defaultResponse;

        public StubRequestHandler(Response defaultResponse)
        {
            _defaultResponse = defaultResponse;
        }

        public Task<Response> Handle(Request requestData, CancellationToken cancellationToken) => Task.FromResult(_defaultResponse);
    }
}
