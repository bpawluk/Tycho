using Tycho.Events;
using Tycho.UnitTests._Data.Events;

namespace Tycho.UnitTests._Data.Handlers;

internal class TestEventOtherHandler : IEventHandler<TestEvent>
{
    public Task HandleAsync(EventContext<TestEvent> context, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
