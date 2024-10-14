using Tycho.Events;
using Tycho.UnitTests._Data.Events;

namespace Tycho.UnitTests._Data.Handlers;

internal class OtherEventHandler : IEventHandler<OtherEvent>
{
    public Task Handle(OtherEvent eventData, CancellationToken cancellationToken) =>
        Task.CompletedTask;
}
