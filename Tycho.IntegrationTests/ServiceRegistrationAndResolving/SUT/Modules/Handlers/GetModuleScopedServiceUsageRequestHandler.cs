﻿using Microsoft.Extensions.DependencyInjection;
using Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Services;
using Tycho.Requests;

namespace Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Modules.Handlers;

internal class GetModuleScopedServiceUsageRequestHandler(IServiceProvider serviceProvider)
    : IRequestHandler<GetModuleScopedServiceUsageRequest, int>
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public Task<int> Handle(GetModuleScopedServiceUsageRequest requestData, CancellationToken cancellationToken)
    {
        var firstServiceInstance = _serviceProvider.GetRequiredService<IScopedService>();
        _ = firstServiceInstance.NumberOfCalls;

        var secondServiceInstance = _serviceProvider.GetRequiredService<IScopedService>();
        var secondNumberOfCalls = secondServiceInstance.NumberOfCalls;

        return Task.FromResult(secondNumberOfCalls);
    }
}