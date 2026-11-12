using Tycho.Events;

namespace Tycho.TestUtils.Events;

public record TestEventWithRequiredData : IEvent
{
    public required string Name { get; init; }
}
