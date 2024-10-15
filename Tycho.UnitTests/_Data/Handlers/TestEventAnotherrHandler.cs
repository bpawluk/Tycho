using Tycho.Events;
using Tycho.UnitTests._Data.Events;

namespace Tycho.UnitTests._Data.Handlers;

internal class TestEventAnotherHandler : IEventHandler<TestEvent>
{
    public Task Handle(TestEvent eventData, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}