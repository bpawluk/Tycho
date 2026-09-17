using Microsoft.Extensions.DependencyInjection;
using Tycho.Apps;
using Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithDownstreamContract.Handlers;
using Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithDownstreamContract.Requests;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithDownstreamContract;

[TychoDefinition]
public class TestApp : TychoApp
{
    protected override void DefineContract(IAppContract app)
    {
        app.Expects<GetItemQuery, GetItemQuery.Result>().HandlesWith<GetItemQueryHandler>();
        app.Expects<DeleteItemCommand>().HandlesWith<DeleteItemCommandHandler>();
    }
    protected override void DefineEvents(IAppEvents app) { }
    protected override void IncludeModules(IAppStructure app) { }
    protected override void RegisterServices(IServiceCollection app) { }
}
