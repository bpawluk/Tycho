namespace Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Services;

internal interface IScopedService
{
    int NumberOfCalls { get; }
}

internal class ScopedService : IScopedService
{
    private int _numberOfCalls;

    public int NumberOfCalls => ++_numberOfCalls;
}