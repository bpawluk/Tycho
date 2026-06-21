using Microsoft.Extensions.DependencyInjection;
using Tycho.Apps;
using Tycho.IntegrationTests.UsingGenericAppsAndModules.SUT.Contract;
using Tycho.IntegrationTests.UsingGenericAppsAndModules.SUT.Handlers;
using Tycho.IntegrationTests.UsingGenericAppsAndModules.SUT.Modules;
using Tycho.IntegrationTests.UsingGenericAppsAndModules.SUT.Modules.Contract;
using Tycho.Requests;

namespace Tycho.IntegrationTests.UsingGenericAppsAndModules.SUT;

// Handles
public sealed record AppWorkflowRequest(TestResult Result) : IRequest<string>;

[TychoDefinition]
public class TestApp<TInput, TOutput> : TychoApp
    where TInput : IInput, new()
    where TOutput : IOutput, new()
{
    protected override void DefineContract(IAppContract app)
    {
        app.Handles<AppWorkflowRequest, string, AppWorkflowRequestHandler<TInput, TOutput>>()
           .Forwards<ModuleWorkflowRequest, string, TestModule<ModuleInput, ModuleOutput>>();
    }

    protected override void DefineEvents(IAppEvents app) { }

    protected override void IncludeModules(IAppStructure app)
    {
        app.Uses<TestModule<ModuleInput, ModuleOutput>>();
        app.Uses<TestModule<ModuleOtherInput, ModuleOtherOutput>>();
    }

    protected override void RegisterServices(IServiceCollection app) { }
}
