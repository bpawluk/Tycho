using Microsoft.Extensions.DependencyInjection;
using Tycho.Apps;
using Tycho.Events;
using Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Handlers;
using Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Modules;
using Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Services;
using Tycho.IntegrationTests._Utils;
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

[TychoDefinition]
public class TestApp(TestWorkflow<TestResult> testWorkflow) : TychoApp
{
    private readonly TestWorkflow<TestResult> _testWorkflow = testWorkflow;

    protected override void DefineContract(IAppContract app)
    {
        app.Expects<BeginTestWorkflowRequest>()
           .HandlesWith<BeginTestWorkflowRequestHandler>();

        app.Expects<GetAppSingletonServiceUsageRequest, int>()
           .HandlesWith<GetAppSingletonServiceUsageRequestHandler>();

        app.Expects<GetAppScopedServiceUsageRequest, int>()
           .HandlesWith<GetAppScopedServiceUsageRequestHandler>();

        app.Expects<GetAppTransientServiceUsageRequest, int>()
           .HandlesWith<GetAppTransientServiceUsageRequestHandler>();

        app.Expects<GetModuleSingletonServiceUsageRequest, int>()
           .ForwardsTo<TestModule>();

        app.Expects<GetModuleScopedServiceUsageRequest, int>()
           .ForwardsTo<TestModule>();

        app.Expects<GetModuleTransientServiceUsageRequest, int>()
           .ForwardsTo<TestModule>();
    }

    protected override void DefineEvents(IAppEvents app)
    {
        app.Expects<GetAppSingletonServiceUsageEvent>()
           .HandlesWith<GetAppSingletonServiceUsageEventHandler>();

        app.Expects<GetAppScopedServiceUsageEvent>()
           .HandlesWith<GetAppScopedServiceUsageEventHandler>();

        app.Expects<GetAppTransientServiceUsageEvent>()
           .HandlesWith<GetAppTransientServiceUsageEventHandler>();

        app.Expects<GetModuleSingletonServiceUsageEvent>()
           .ForwardsTo<TestModule>();

        app.Expects<GetModuleScopedServiceUsageEvent>()
           .ForwardsTo<TestModule>();

        app.Expects<GetModuleTransientServiceUsageEvent>()
           .ForwardsTo<TestModule>();
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
