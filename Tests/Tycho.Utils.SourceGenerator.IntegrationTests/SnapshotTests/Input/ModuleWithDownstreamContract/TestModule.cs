using Microsoft.Extensions.DependencyInjection;
using Tycho.Modules;
using Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithDownstreamContract.Handlers;
using Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithDownstreamContract.Requests;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithDownstreamContract;

[TychoDefinition]
public class TestModule : TychoModule
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Expects<GetItemQuery, GetItemQuery.Result>().HandlesWith<GetItemQueryHandler>();
        module.Expects<DeleteItemCommand>().HandlesWith<DeleteItemCommandHandler>();
    }
    protected override void DefineEvents(IModuleEvents module) { }
    protected override void IncludeModules(IModuleStructure module) { }
    protected override void RegisterServices(IServiceCollection module) { }
}
