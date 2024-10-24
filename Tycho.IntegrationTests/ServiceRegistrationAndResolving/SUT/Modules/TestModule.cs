using Microsoft.Extensions.DependencyInjection;
using Tycho.Events;
using Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Modules.Handlers;
using Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Services;
using Tycho.Modules;
using Tycho.Requests;

namespace Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Modules;

// Handles
public record GetModuleSingletonServiceUsageRequest : IRequest<int>;
public record GetModuleScopedServiceUsageRequest : IRequest<int>;
public record GetModuleTransientServiceUsageRequest : IRequest<int>;

// Requires
public record EndTestWorkflowRequest(TestResult Result) : IRequest;

// Events
public record GetModuleSingletonServiceUsageEvent(TestResult Result) : IEvent;
public record GetModuleScopedServiceUsageEvent(TestResult Result) : IEvent;
public record GetModuleTransientServiceUsageEvent(TestResult Result) : IEvent;

internal class TestModule : TychoModule
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Handles<GetModuleSingletonServiceUsageRequest, int, GetModuleSingletonServiceUsageRequestHandler>()
              .Handles<GetModuleScopedServiceUsageRequest, int, GetModuleScopedServiceUsageRequestHandler>()
              .Handles<GetModuleTransientServiceUsageRequest, int, GetModuleTransientServiceUsageRequestHandler>();

        module.Requires<EndTestWorkflowRequest>();
    }

    protected override void IncludeModules(IModuleStructure module) { }

    protected override void MapEvents(IModuleEvents module) 
    {
        module.Handles<GetModuleSingletonServiceUsageEvent, GetModuleSingletonServiceUsageEventHandler>()
              .Handles<GetModuleScopedServiceUsageEvent, GetModuleScopedServiceUsageEventHandler>()
              .Handles<GetModuleTransientServiceUsageEvent, GetModuleTransientServiceUsageEventHandler>();
    }

    protected override void RegisterServices(IServiceCollection module)
    {
        module.AddSingleton<ISingletonService, SingletonService>()
              .AddScoped<IScopedService, ScopedService>()
              .AddTransient<ITransientService, TransientService>();
    }
}