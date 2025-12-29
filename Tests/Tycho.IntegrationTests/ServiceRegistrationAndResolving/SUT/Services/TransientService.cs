namespace Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Services;

internal interface ITransientService
{
    int NumberOfCalls { get; }
}

internal class TransientService : ITransientService
{
    private int _numberOfCalls;

    public int NumberOfCalls => ++_numberOfCalls;
}