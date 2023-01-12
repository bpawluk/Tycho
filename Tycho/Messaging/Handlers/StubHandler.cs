using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Payload;

namespace Tycho.Messaging.Handlers;

internal class StubEventHandler<Event> : IEventHandler<Event>
    where Event : class, IEvent
{
    public Task Handle(Event eventData, CancellationToken cancellationToken = default) => Task.CompletedTask;
}

internal class StubCommandHandler<Command> : ICommandHandler<Command>
    where Command : class, ICommand
{
    public Task Handle(Command commandData, CancellationToken cancellationToken = default) => Task.CompletedTask;
}

internal class StubQueryHandler<Query, Response> : IQueryHandler<Query, Response>
    where Query : class, IQuery<Response>
{
    private readonly Response _defaultResponse;

    public StubQueryHandler(Response defaultResponse)
    {
        _defaultResponse = defaultResponse;
    }

    public Task<Response> Handle(Query commandData, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<Response>(_defaultResponse);
    }
}
