using Microsoft.Extensions.DependencyInjection;
using Tycho.IntegrationTests.UsingGenericAppsAndModules.SUT.Contract;
using Tycho.IntegrationTests.UsingGenericAppsAndModules.SUT.Modules.Handlers;
using Tycho.Modules;
using Tycho.Requests;

namespace Tycho.IntegrationTests.UsingGenericAppsAndModules.SUT.Modules;

// Handles
public sealed record ModuleWorkflowRequest(TestResult Result) : IRequest<string>;

[TychoDefinition]
public class TestModule<TInput, TOutput> : TychoModule
    where TInput : IInput, new()
    where TOutput : IOutput, new()
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Handles<ModuleWorkflowRequest, string, ModuleWorkflowRequestHandler<TInput, TOutput>>();
    }

    protected override void DefineEvents(IModuleEvents module) { }

    protected override void IncludeModules(IModuleStructure module) { }

    protected override void RegisterServices(IServiceCollection module) { }
}
