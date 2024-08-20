using Microsoft.Extensions.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Handlers;

namespace IntegrationTests.ProvidingConfiguration.SUT.Handlers;

internal class GetConfiguredValueViaIConfigurationRequestHandler : IRequestHandler<GetConfiguredValueViaIConfigurationRequest, DateTime>
{
    private readonly IConfiguration _config;

    public GetConfiguredValueViaIConfigurationRequestHandler(IConfiguration config)
    {
        _config = config;
    }

    public Task<DateTime> Handle(GetConfiguredValueViaIConfigurationRequest requestData, CancellationToken cancellationToken)
    {
        return Task.FromResult(DateTime.Parse(_config["SomeDate"]!));
    }
}