namespace Tycho.IntegrationTests.SendingRequestsHorizontally;

public record TestResult
{
    public string Id { get; init; } = default!;

    public int HandlingCount { get; set; }
}
