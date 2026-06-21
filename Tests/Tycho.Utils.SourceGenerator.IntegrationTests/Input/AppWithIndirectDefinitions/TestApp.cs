using Microsoft.Extensions.DependencyInjection;
using Tycho.Apps;
using Tycho.Events;
using Tycho.Requests;
using Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithIndirectDefinitions.Handlers;
using Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithIndirectDefinitions.Helpers;
using Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithIndirectDefinitions.Modules;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithIndirectDefinitions;

public record TestRequestFromLocalHelper : IRequest<string>;
public record TestRequestFromLocalStaticHelper : IRequest<string>;
public record TestRequestFromHelperClass : IRequest<string>;
public record TestRequestFromHelperStaticClass : IRequest<string>;
public record TestRequestFromHelperExtension : IRequest<string>;

public record TestEventFromLocalHelper : IEvent;
public record TestEventFromLocalStaticHelper : IEvent;
public record TestEventFromHelperClass : IEvent;
public record TestEventFromHelperStaticClass : IEvent;
public record TestEventFromHelperExtension : IEvent;

[TychoDefinition]
public class TestApp : TychoApp
{
    protected override void DefineContract(IAppContract app)
    {
        DefineContractFromLocalHelper(app);
        DefineContractFromLocalStaticHelper(app);

        var helperClass = new HelperClass();
        helperClass.DefineContract(app);
        HelperStaticClass.DefineContract(app);
        app.DefineContractFromExtension();
    }

    protected override void DefineEvents(IAppEvents app)
    {
        DefineEventsFromLocalHelper(app);
        DefineEventsFromLocalStaticHelper(app);

        var helperClass = new HelperClass();
        helperClass.DefineEvents(app);
        HelperStaticClass.DefineEvents(app);
        app.DefineEventsFromExtension();
    }

    protected override void IncludeModules(IAppStructure app)
    {
        IncludeModulesFromLocalHelper(app);
        IncludeModulesFromLocalStaticHelper(app);

        var helperClass = new HelperClass();
        helperClass.IncludeModules(app);
        HelperStaticClass.IncludeModules(app);
        app.IncludeModulesFromExtension();
    }

    protected override void RegisterServices(IServiceCollection app) { }

    private void DefineContractFromLocalHelper(IAppContract app)
    {
        app.Handles<TestRequestFromLocalHelper, string, TestRequestHandler>();
    }

    private void DefineEventsFromLocalHelper(IAppEvents app)
    {
        app.Handles<TestEventFromLocalHelper, TestEventHandler>();
    }

    private void IncludeModulesFromLocalHelper(IAppStructure app)
    {
        app.Uses<LocalHelperModule>();
    }

    private static void DefineContractFromLocalStaticHelper(IAppContract app)
    {
        app.Handles<TestRequestFromLocalStaticHelper, string, TestRequestHandler>();
    }

    private static void DefineEventsFromLocalStaticHelper(IAppEvents app)
    {
        app.Handles<TestEventFromLocalStaticHelper, TestEventHandler>();
    }

    private static void IncludeModulesFromLocalStaticHelper(IAppStructure app)
    {
        app.Uses<LocalStaticHelperModule>();
    }
}
