using Tycho.Apps;
using Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions;
using Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions.Handlers;
using Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions.Modules;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions.Helpers;

internal static class HelperStaticClass
{
    public static void DefineContract(IAppContract app)
    {
        app.Expects<TestRequestFromHelperStaticClass, string>().HandlesWith<TestRequestHandler>();
    }

    public static void DefineContractFromExtension(this IAppContract app)
    {
        app.Expects<TestRequestFromHelperExtension, string>().HandlesWith<TestRequestHandler>();
    }

    public static void DefineEvents(IAppEvents app)
    {
        app.Expects<TestEventFromHelperStaticClass>().HandlesWith<TestEventHandler>();
    }

    public static void DefineEventsFromExtension(this IAppEvents app)
    {
        app.Expects<TestEventFromHelperExtension>().HandlesWith<TestEventHandler>();
    }

    public static void IncludeModules(IAppStructure app)
    {
        app.Uses<HelperStaticClassModule>();
    }

    public static void IncludeModulesFromExtension(this IAppStructure app)
    {
        app.Uses<HelperExtensionModule>();
    }
}
