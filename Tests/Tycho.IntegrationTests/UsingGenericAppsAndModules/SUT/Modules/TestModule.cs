using Microsoft.Extensions.DependencyInjection;
using Tycho.IntegrationTests.UsingGenericAppsAndModules.SUT.Modules.Handlers;
using Tycho.Modules;

namespace Tycho.IntegrationTests.UsingGenericAppsAndModules.SUT.Modules;

[TychoDefinition]
public partial class TestModule<TPayload, TKey> : TychoModule
    where TPayload : PayloadBase, IMarker, new()
    where TKey : notnull
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Handles<ModuleWorkflowRequest, string, ModuleWorkflowRequestHandler<TPayload, TKey>>()
              .Requires<CompleteWorkflowRequest, string>();
    }

    protected override void DefineEvents(IModuleEvents module) { }

    protected override void IncludeModules(IModuleStructure module) { }

    protected override void RegisterServices(IServiceCollection module) { }
}
