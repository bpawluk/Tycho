namespace Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Services;

internal interface ISingletonService
{
    int NumberOfCalls { get; }
}

internal class SingletonService : ISingletonService
{
    public int NumberOfCalls => ++field;
}
