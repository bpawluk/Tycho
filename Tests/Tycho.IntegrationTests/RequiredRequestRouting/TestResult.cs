namespace Tycho.IntegrationTests.RequiredRequestRouting;

public sealed class TestResult
{
    public List<Invocation> Invocations { get; } = [];
}

public sealed record Invocation(string Destination, string Value);
