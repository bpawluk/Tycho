using Tycho.Events;

namespace Tycho.Persistence.EFCore.UnitTests._Data.Events;

public record TestEventWithRequiredData : IEvent
{
    public required string Name { get; init; }
}