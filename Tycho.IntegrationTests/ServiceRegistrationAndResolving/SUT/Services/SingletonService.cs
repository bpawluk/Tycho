namespace Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Services;

internal interface ISingletonService
{
    int NumberOfCalls { get; }
}

internal class SingletonService : ISingletonService
{
    private int _numberOfCalls;

    public int NumberOfCalls => ++_numberOfCalls;
}