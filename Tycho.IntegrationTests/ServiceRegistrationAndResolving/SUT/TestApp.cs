using Microsoft.Extensions.DependencyInjection;
using Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Handlers;
using Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Services;
using Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Submodules.TestModule;
using TychoV2.Apps;
using TychoV2.Requests;

namespace Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT;

// Handles
public record GetAppSingletonServiceUsageRequest : IRequest<int>;
public record GetAppScopedServiceUsageRequest : IRequest<int>;
public record GetAppTransientServiceUsageRequest : IRequest<int>;

public class TestApp : TychoApp
{
    protected override void DefineContract(IAppContract app)
    {
        app.Handles<GetAppSingletonServiceUsageRequest, int, GetAppSingletonServiceUsageRequestHandler>()
           .Handles<GetAppScopedServiceUsageRequest, int, GetAppScopedServiceUsageRequestHandler>()
           .Handles<GetAppTransientServiceUsageRequest, int, GetAppTransientServiceUsageRequestHandler>();

        app.Forwards<GetModuleSingletonServiceUsageRequest, int, TestModule>()
           .Forwards<GetModuleScopedServiceUsageRequest, int, TestModule>()
           .Forwards<GetModuleTransientServiceUsageRequest, int, TestModule>();
    }

    protected override void IncludeModules(IAppStructure app)
    {
        app.AddModule<TestModule>();
    }

    protected override void MapEvents(IAppEvents app) { }

    protected override void RegisterServices(IServiceCollection app)
    {
        app.AddSingleton<ISingletonService, SingletonService>()
           .AddScoped<IScopedService, ScopedService>()
           .AddTransient<ITransientService, TransientService>();
    }
}
