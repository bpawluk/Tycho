using Tycho.Events;
using Tycho.UnitTests._Data.Events;

namespace Tycho.UnitTests._Data.Handlers;

internal class MultiEventHandler : IEventHandler<TestEvent>, IEventHandler<OtherEvent>, IEventHandler<AnotherEvent>
{
    public Task HandleAsync(EventContext<TestEvent> context, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task HandleAsync(EventContext<OtherEvent> context, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task HandleAsync(EventContext<AnotherEvent> context, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
