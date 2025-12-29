namespace Tycho.IntegrationTests.ServiceRegistrationAndResolving;

public record TestResult
{
    public string Id { get; init; } = default!;

    public int NumberOfCalls { get; set; } = 0;
}