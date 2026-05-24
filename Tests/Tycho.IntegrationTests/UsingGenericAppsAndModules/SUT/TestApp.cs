using Microsoft.Extensions.DependencyInjection;
using Tycho.Apps;
using Tycho.IntegrationTests.UsingGenericAppsAndModules.SUT.Handlers;
using Tycho.IntegrationTests.UsingGenericAppsAndModules.SUT.Modules;

namespace Tycho.IntegrationTests.UsingGenericAppsAndModules.SUT;

[TychoDefinition]
public partial class TestApp<TPayload, TKey> : TychoApp
    where TPayload : PayloadBase, IMarker, new()
    where TKey : notnull
{
    protected override void DefineContract(IAppContract app)
    {
        app.Handles<AppWorkflowRequest, string, AppWorkflowRequestHandler<TPayload, TKey>>()
           .Forwards<ModuleWorkflowRequest, string, TestModule<TPayload, TKey>>();
    }

    protected override void DefineEvents(IAppEvents app) { }

    protected override void IncludeModules(IAppStructure app)
    {
        app.Uses<TestModule<TPayload, TKey>>(contract =>
        {
            contract.Handle<CompleteWorkflowRequest, string, CompleteWorkflowRequestHandler<TPayload, TKey>>();
        });
    }

    protected override void RegisterServices(IServiceCollection app) { }
}
