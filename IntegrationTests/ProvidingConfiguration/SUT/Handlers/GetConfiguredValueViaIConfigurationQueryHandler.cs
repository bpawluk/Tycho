using Microsoft.Extensions.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Handlers;

namespace IntegrationTests.ProvidingConfiguration.SUT.Handlers;

internal class GetConfiguredValueViaIConfigurationQueryHandler : IRequestHandler<GetConfiguredValueViaIConfigurationQuery, DateTime>
{
    private readonly IConfiguration _config;

    public GetConfiguredValueViaIConfigurationQueryHandler(IConfiguration config)
    {
        _config = config;
    }

    public Task<DateTime> Handle(GetConfiguredValueViaIConfigurationQuery queryData, CancellationToken cancellationToken)
    {
        return Task.FromResult(DateTime.Parse(_config["SomeDate"]!));
    }
}