using Microsoft.Extensions.DependencyInjection;
using Tycho.Modules;
using Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithDownstreamContract.Handlers;
using Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithDownstreamContract.Requests;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithDownstreamContract;

[TychoDefinition]
public partial class TestModule : TychoModule
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Handles<GetItemQuery, GetItemQuery.Result, GetItemQueryHandler>();
        module.Handles<DeleteItemCommand, DeleteItemCommandHandler>();
    }
    protected override void DefineEvents(IModuleEvents module) { }
    protected override void IncludeModules(IModuleStructure module) { }
    protected override void RegisterServices(IServiceCollection module) { }
}
