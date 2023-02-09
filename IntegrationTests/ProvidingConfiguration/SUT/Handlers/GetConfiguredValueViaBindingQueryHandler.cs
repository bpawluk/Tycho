using IntegrationTests.ProvidingConfiguration.SUT.Configuration;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Handlers;

namespace IntegrationTests.ProvidingConfiguration.SUT.Handlers;

internal class GetConfiguredValueViaBindingQueryHandler : IQueryHandler<GetConfiguredValueViaBindingQuery, int>
{
    private readonly TestConfig _config;

    public GetConfiguredValueViaBindingQueryHandler(TestConfig config)
    {
        _config = config;
    }

    public Task<int> Handle(GetConfiguredValueViaBindingQuery queryData, CancellationToken cancellationToken)
    {
        return Task.FromResult(_config.SomeNumber);
    }
}