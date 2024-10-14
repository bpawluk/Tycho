using Tycho.Events;
using Tycho.UnitTests._Data.Events;

namespace Tycho.UnitTests._Data.Handlers;

internal class AnotherEventHandler : IEventHandler<AnotherEvent>
{
    public Task Handle(AnotherEvent eventData, CancellationToken cancellationToken) =>
        Task.CompletedTask;
}
