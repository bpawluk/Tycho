using Microsoft.Extensions.DependencyInjection;
using Tycho.Apps;
using Tycho.Events;
using Tycho.IntegrationTests._Utils;
using Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Handlers;
using Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Modules;
using Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Services;
using Tycho.Requests;

namespace Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT;

// Handles
public record BeginTestWorkflowRequest(TestResult Result) : IRequest;
public record GetAppSingletonServiceUsageRequest : IRequest<int>;
public record GetAppScopedServiceUsageRequest : IRequest<int>;
public record GetAppTransientServiceUsageRequest : IRequest<int>;

// Events
public record GetAppSingletonServiceUsageEvent(TestResult Result) : IEvent;
public record GetAppScopedServiceUsageEvent(TestResult Result) : IEvent;
public record GetAppTransientServiceUsageEvent(TestResult Result) : IEvent;

public class TestApp(TestWorkflow<TestResult> testWorkflow) : TychoApp
{
    private readonly TestWorkflow<TestResult> _testWorkflow = testWorkflow;

    protected override void DefineContract(IAppContract app)
    {
        app.Handles<GetAppSingletonServiceUsageRequest, int, GetAppSingletonServiceUsageRequestHandler>()
           .Handles<GetAppScopedServiceUsageRequest, int, GetAppScopedServiceUsageRequestHandler>()
           .Handles<GetAppTransientServiceUsageRequest, int, GetAppTransientServiceUsageRequestHandler>();

        app.Forwards<GetModuleSingletonServiceUsageRequest, int, TestModule>()
           .Forwards<GetModuleScopedServiceUsageRequest, int, TestModule>()
           .Forwards<GetModuleTransientServiceUsageRequest, int, TestModule>();

        app.Handles<BeginTestWorkflowRequest, BeginTestWorkflowRequestHandler>();
    }

    protected override void DefineEvents(IAppEvents app)
    {
        app.Handles<GetAppSingletonServiceUsageEvent, GetAppSingletonServiceUsageEventHandler>()
           .Handles<GetAppScopedServiceUsageEvent, GetAppScopedServiceUsageEventHandler>()
           .Handles<GetAppTransientServiceUsageEvent, GetAppTransientServiceUsageEventHandler>();

        app.Routes<GetModuleSingletonServiceUsageEvent>()
           .Forwards<TestModule>();

        app.Routes<GetModuleScopedServiceUsageEvent>()
           .Forwards<TestModule>();

        app.Routes<GetModuleTransientServiceUsageEvent>()
           .Forwards<TestModule>();
    }

    protected override void IncludeModules(IAppStructure app)
    {
        app.Uses<TestModule>(contract =>
        {
            contract.Handle<EndTestWorkflowRequest, EndTestWorkflowRequestHandler>();
        });
    }

    protected override void RegisterServices(IServiceCollection app)
    {
        app.AddSingleton(_testWorkflow)
           .AddSingleton<ISingletonService, SingletonService>()
           .AddScoped<IScopedService, ScopedService>()
           .AddTransient<ITransientService, TransientService>();
    }
}