using Microsoft.Extensions.DependencyInjection;
using Tycho.Apps;
using Tycho.Events;
using Tycho.IntegrationTests.UsingIndirectDefinitions.SUT.Handlers;
using Tycho.IntegrationTests.UsingIndirectDefinitions.SUT.Helpers;
using Tycho.IntegrationTests.UsingIndirectDefinitions.SUT.Modules;
using Tycho.Requests;

namespace Tycho.IntegrationTests.UsingIndirectDefinitions.SUT;

// Handles
public record TestRequestFromLocalHelper : IRequest<string>;
public record TestRequestFromLocalStaticHelper : IRequest<string>;
public record TestRequestFromHelperClass : IRequest<string>;
public record TestRequestFromHelperStaticClass : IRequest<string>;
public record TestRequestFromHelperExtension : IRequest<string>;
public record TestRequestUsingLocalStructureModule : IRequest<string>;
public record TestRequestUsingLocalStaticStructureModule : IRequest<string>;
public record TestRequestUsingHelperClassStructureModule : IRequest<string>;
public record TestRequestUsingHelperStaticStructureModule : IRequest<string>;
public record TestRequestUsingHelperExtensionStructureModule : IRequest<string>;

// Events
public record TestEventFromLocalHelper() : IEvent;
public record TestEventFromLocalStaticHelper() : IEvent;
public record TestEventFromHelperClass() : IEvent;
public record TestEventFromHelperStaticClass() : IEvent;
public record TestEventFromHelperExtension() : IEvent;

[TychoDefinition]
public partial class TestApp : TychoApp
{
    protected override void DefineContract(IAppContract app)
    {
        IAppContractHelperLocalMethod(app);
        IAppContractHelperLocalStaticMethod(app);
        var helperClass = new HelperClass();
        helperClass.IAppContractHelperMethod(app);
        HelperStaticClass.IAppContractHelperStaticMethod(app);
        app.IAppContractHelperExtension();

        app.Handles<TestRequestUsingLocalStructureModule, string, TestRequestUsingLocalStructureModuleHandler>();
        app.Handles<TestRequestUsingLocalStaticStructureModule, string, TestRequestUsingLocalStaticStructureModuleHandler>();
        app.Handles<TestRequestUsingHelperClassStructureModule, string, TestRequestUsingHelperClassStructureModuleHandler>();
        app.Handles<TestRequestUsingHelperStaticStructureModule, string, TestRequestUsingHelperStaticStructureModuleHandler>();
        app.Handles<TestRequestUsingHelperExtensionStructureModule, string, TestRequestUsingHelperExtensionStructureModuleHandler>();
    }

    protected override void DefineEvents(IAppEvents app)
    {
        IAppEventsHelperLocalMethod(app);
        IAppEventsHelperLocalStaticMethod(app);
        var helperClass = new HelperClass();
        helperClass.IAppEventsHelperMethod(app);
        HelperStaticClass.IAppEventsHelperStaticMethod(app);
        app.IAppEventsHelperExtension();
    }

    protected override void IncludeModules(IAppStructure app)
    {
        IAppStructureHelperLocalMethod(app);
        IAppStructureHelperLocalStaticMethod(app);
        var helperClass = new HelperClass();
        helperClass.IAppStructureHelperMethod(app);
        HelperStaticClass.IAppStructureHelperStaticMethod(app);
        app.IAppStructureHelperExtension();
    }

    protected override void RegisterServices(IServiceCollection app)
    {
        app.AddSingleton(Configuration);
    }

#pragma warning disable CA1822

    private void IAppContractHelperLocalMethod(IAppContract app)
    {
        app.Handles<TestRequestFromLocalHelper, string, TestRequestHandler>();
    }

    private void IAppEventsHelperLocalMethod(IAppEvents app)
    {
        app.Handles<TestEventFromLocalHelper, TestEventHandler>();
    }

    private void IAppStructureHelperLocalMethod(IAppStructure app)
    {
        app.Uses<LocalHelperModule>();
    }

#pragma warning restore CA1822

    private static void IAppContractHelperLocalStaticMethod(IAppContract app)
    {
        app.Handles<TestRequestFromLocalStaticHelper, string, TestRequestHandler>();
    }

    private static void IAppEventsHelperLocalStaticMethod(IAppEvents app)
    {
        app.Handles<TestEventFromLocalStaticHelper, TestEventHandler>();
    }

    private static void IAppStructureHelperLocalStaticMethod(IAppStructure app)
    {
        app.Uses<LocalStaticHelperModule>();
    }
}
