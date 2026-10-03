namespace Tycho.IntegrationTests.ForwardingEventsVertically;

public record TestResult
{
    public string Id { get; init; } = default!;
    public string Value { get; init; } = string.Empty;
    public string? LeafValue { get; init; }
}
