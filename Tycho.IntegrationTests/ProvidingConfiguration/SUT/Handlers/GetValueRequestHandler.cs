using Microsoft.Extensions.Configuration;
using Tycho.IntegrationTests.ProvidingConfiguration.SUT.Modules;
using TychoV2.Requests;

namespace Tycho.IntegrationTests.ProvidingConfiguration.SUT.Handlers;

internal class GetValueRequestHandler(IConfiguration config)
    : IHandle<GetAlphaValueRequest, string>
    , IHandle<GetBetaValueRequest, string>
{
    private readonly IConfiguration _config = config;

    private string Value => _config["Value"]!;

    public Task<string> Handle(GetAlphaValueRequest requestData, CancellationToken cancellationToken) =>
        Task.FromResult(Value);

    public Task<string> Handle(GetBetaValueRequest requestData, CancellationToken cancellationToken) =>
        Task.FromResult(Value);

}