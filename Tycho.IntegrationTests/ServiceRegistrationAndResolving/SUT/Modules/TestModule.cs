using Microsoft.Extensions.DependencyInjection;
using Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Handlers;
using Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Services;
using Tycho.Modules;
using Tycho.Requests;

namespace Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Modules;

// Handles
public record GetModuleSingletonServiceUsageRequest : IRequest<int>;
public record GetModuleScopedServiceUsageRequest : IRequest<int>;
public record GetModuleTransientServiceUsageRequest : IRequest<int>;

internal class TestModule : TychoModule
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Handles<GetModuleSingletonServiceUsageRequest, int, GetSingletonServiceUsageRequestHandler>()
              .Handles<GetModuleScopedServiceUsageRequest, int, GetScopedServiceUsageRequestHandler>()
              .Handles<GetModuleTransientServiceUsageRequest, int, GetTransientServiceUsageRequestHandler>();
    }

    protected override void IncludeModules(IModuleStructure module) { }

    protected override void MapEvents(IModuleEvents module) { }

    protected override void RegisterServices(IServiceCollection module)
    {
        module.AddSingleton<ISingletonService, SingletonService>()
              .AddScoped<IScopedService, ScopedService>()
              .AddTransient<ITransientService, TransientService>();
    }
}