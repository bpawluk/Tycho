namespace Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Services;

internal interface ITransientService
{
    int NumberOfCalls { get; }
}

internal class TransientService : ITransientService
{
    private int _numberOfCalls = 0;
    public int NumberOfCalls => ++_numberOfCalls;
}
