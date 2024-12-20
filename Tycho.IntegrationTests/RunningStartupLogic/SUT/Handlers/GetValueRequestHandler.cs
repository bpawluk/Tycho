﻿using Tycho.IntegrationTests.RunningStartupLogic.SUT.Modules;
using Tycho.IntegrationTests.RunningStartupLogic.SUT.Services;
using Tycho.Requests;

namespace Tycho.IntegrationTests.RunningStartupLogic.SUT.Handlers;

internal class GetValueRequestHandler(TestService testService)
    : IRequestHandler<GetAppValueRequest, string>
    , IRequestHandler<GetModuleValueRequest, string>
{
    private readonly TestService _testService = testService;

    public Task<string> Handle(GetAppValueRequest requestData, CancellationToken cancellationToken)
    {
        return Task.FromResult(_testService.Value!);
    }

    public Task<string> Handle(GetModuleValueRequest requestData, CancellationToken cancellationToken)
    {
        return Task.FromResult(_testService.Value!);
    }
}