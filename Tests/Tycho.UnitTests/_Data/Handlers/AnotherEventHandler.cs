using Tycho.Events;
using Tycho.UnitTests._Data.Events;

namespace Tycho.UnitTests._Data.Handlers;

internal class AnotherEventHandler : IEventHandler<AnotherEvent>
{
    public Task Handle(EventContext<AnotherEvent> context, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}