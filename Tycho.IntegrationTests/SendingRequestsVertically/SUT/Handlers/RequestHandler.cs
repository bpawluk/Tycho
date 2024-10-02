﻿using Tycho.IntegrationTests.SendingRequestsVertically.SUT.Modules.Alpha;
using TychoV2.Modules;
using TychoV2.Requests;

namespace Tycho.IntegrationTests.SendingRequestsVertically.SUT.Handlers;

internal class RequestHandler(IModule<AlphaModule> alphaModule)
    : IHandle<Request>
    , IHandle<RequestWithResponse, string>
{
    private readonly IModule<AlphaModule> _alphaModule = alphaModule;

    public Task Handle(Request requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return _alphaModule.Execute(new AlphaInRequest(requestData.Result), cancellationToken);
    }

    public Task<string> Handle(RequestWithResponse requestData, CancellationToken cancellationToken)
    {
        requestData.Result.HandlingCount++;
        return _alphaModule.Execute<AlphaInRequestWithResponse, string>(
            new AlphaInRequestWithResponse(requestData.Result), 
            cancellationToken);
    }
}
