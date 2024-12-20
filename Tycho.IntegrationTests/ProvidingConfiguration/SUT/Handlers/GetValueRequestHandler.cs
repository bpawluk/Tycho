﻿using Microsoft.Extensions.Configuration;
using Tycho.IntegrationTests.ProvidingConfiguration.SUT.Modules;
using Tycho.Requests;

namespace Tycho.IntegrationTests.ProvidingConfiguration.SUT.Handlers;

internal class GetValueRequestHandler(IConfiguration config)
    : IRequestHandler<GetAppValueRequest, string>
    , IRequestHandler<GetAlphaValueRequest, string>
    , IRequestHandler<GetBetaValueRequest, string>
{
    private readonly IConfiguration _config = config;

    private string Value => _config["Value"]!;

    public Task<string> Handle(GetAppValueRequest requestData, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Value);
    }

    public Task<string> Handle(GetAlphaValueRequest requestData, CancellationToken cancellationToken)
    {
        return Task.FromResult(Value);
    }

    public Task<string> Handle(GetBetaValueRequest requestData, CancellationToken cancellationToken)
    {
        return Task.FromResult(Value);
    }
}