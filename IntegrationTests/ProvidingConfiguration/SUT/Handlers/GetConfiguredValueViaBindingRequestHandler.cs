using IntegrationTests.ProvidingConfiguration.SUT.Configuration;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Handlers;

namespace IntegrationTests.ProvidingConfiguration.SUT.Handlers;

internal class GetConfiguredValueViaBindingRequestHandler(TestConfig config) : 
    IRequestHandler<GetConfiguredValueViaBindingRequest, int>
{
    private readonly TestConfig _config = config;

    public Task<int> Handle(GetConfiguredValueViaBindingRequest requestData, CancellationToken cancellationToken)
    {
        return Task.FromResult(_config.SomeNumber);
    }
}