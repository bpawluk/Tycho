using Microsoft.Extensions.DependencyInjection;
using Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Services;
using Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Submodules.TestModule.Handlers;
using TychoV2.Modules;
using TychoV2.Requests;

namespace Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Submodules.TestModule;

// Handles
public record GetModuleSingletonServiceUsageRequest : IRequest<int>;
public record GetModuleScopedServiceUsageRequest : IRequest<int>;
public record GetModuleTransientServiceUsageRequest : IRequest<int>;

internal class TestModule : TychoModule
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Handles<GetModuleSingletonServiceUsageRequest, int, GetModuleSingletonServiceUsageRequestHandler>()
              .Handles<GetModuleScopedServiceUsageRequest, int, GetModuleScopedServiceUsageRequestHandler>()
              .Handles<GetModuleTransientServiceUsageRequest, int, GetModuleTransientServiceUsageRequestHandler>();
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
