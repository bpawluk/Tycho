using IntegrationTests.ProvidingConfiguration.SUT.Configuration;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Handlers;

namespace IntegrationTests.ProvidingConfiguration.SUT.Handlers;

internal class GetConfiguredValueViaBindingRequestHandler : IRequestHandler<GetConfiguredValueViaBindingRequest, int>
{
    private readonly TestConfig _config;

    public GetConfiguredValueViaBindingRequestHandler(TestConfig config)
    {
        _config = config;
    }

    public Task<int> Handle(GetConfiguredValueViaBindingRequest requestData, CancellationToken cancellationToken)
    {
        return Task.FromResult(_config.SomeNumber);
    }
}