using Tycho.Events;
using Tycho.UnitTests._Data.Events;

namespace Tycho.UnitTests._Data.Handlers;

internal class TestEventHandler : IEventHandler<TestEvent>
{
    public Task Handle(TestEvent eventData, CancellationToken cancellationToken) =>
        Task.CompletedTask;
}
