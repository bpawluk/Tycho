using Tycho.Events;
using Tycho.TestUtils.Events;

namespace Tycho.TestUtils.Handlers;

public class OtherEventHandler : IEventHandler<OtherEvent>
{
    public Task HandleAsync(EventContext<OtherEvent> context, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
