using Tycho.Events;
using Tycho.TestUtils.Events;

namespace Tycho.TestUtils.Handlers;

public class TestEventOtherHandler : IEventHandler<TestEvent>
{
    public Task HandleAsync(EventContext<TestEvent> context, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
