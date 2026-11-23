using Microsoft.Extensions.DependencyInjection;
using Tycho.Modules;
using Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithUpstreamContract.Requests;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithUpstreamContract;

[TychoDefinition]
public class TestModule : TychoModule
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Requires<GetParentDataQuery, GetParentDataQuery.Result>();
        module.Requires<NotifyParentCommand>();
    }
    protected override void DefineEvents(IModuleEvents module) { }
    protected override void IncludeModules(IModuleStructure module) { }
    protected override void RegisterServices(IServiceCollection module) { }
}
