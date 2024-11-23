using Microsoft.Extensions.Configuration;
using Tycho.Requests;

namespace Tycho.IntegrationTests.SettingUpForHostApps.SUT.Handlers;

internal class TestRequestHandler(IConfiguration configuration) : IRequestHandler<TestRequest, string>
{
    private readonly IConfiguration _configuration = configuration;

    public Task<string> Handle(TestRequest requestData, CancellationToken cancellationToken)
    {
        return Task.FromResult(_configuration["Response"]!);
    }
}