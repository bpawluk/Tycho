namespace Tycho.IntegrationTests.SendingRequestsVertically;

public record TestResult
{
    public string Id { get; init; } = default!;

    public int HandlingCount { get; set; }
}
