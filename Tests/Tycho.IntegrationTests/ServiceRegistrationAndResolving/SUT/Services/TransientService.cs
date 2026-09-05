namespace Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Services;

internal interface ITransientService
{
    int NumberOfCalls { get; }
}

internal class TransientService : ITransientService
{
    public int NumberOfCalls => ++field;
}
