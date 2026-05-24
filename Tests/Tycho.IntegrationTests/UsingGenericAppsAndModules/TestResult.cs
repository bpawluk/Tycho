namespace Tycho.IntegrationTests.UsingGenericAppsAndModules;

public sealed record TestResult
{
    public string Id { get; init; } = default!;

    public int HandlingCount { get; set; }

    public string LastHandledBy { get; set; } = string.Empty;
}
