using Tycho.Events;
using Tycho.UnitTests._Data.Events;

namespace Tycho.UnitTests._Data.Handlers;

internal class OtherEventHandler : IEventHandler<OtherEvent>
{
    public Task HandleAsync(EventContext<OtherEvent> context, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
