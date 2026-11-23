using Tycho.Events;
using Tycho.TestUtils.Events;

namespace Tycho.TestUtils.Handlers;

public class AnotherEventHandler : IEventHandler<AnotherEvent>
{
    public Task HandleAsync(EventContext<AnotherEvent> context, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
