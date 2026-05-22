using Tycho.Apps;
using Tycho.IntegrationTests.UsingIndirectDefinitions.SUT.Handlers;
using Tycho.IntegrationTests.UsingIndirectDefinitions.SUT.Modules;

namespace Tycho.IntegrationTests.UsingIndirectDefinitions.SUT.Helpers;

internal static class HelperStaticClass
{
    public static void IAppContractHelperStaticMethod(IAppContract app)
    {
        app.Handles<TestRequestFromHelperStaticClass, string, TestRequestHandler>();
    }

    public static void IAppContractHelperExtension(this IAppContract app)
    {
        app.Handles<TestRequestFromHelperExtension, string, TestRequestHandler>();
    }

    public static void IAppEventsHelperStaticMethod(IAppEvents app)
    {
        app.Handles<TestEventFromHelperStaticClass, TestEventHandler>();
    }

    public static void IAppEventsHelperExtension(this IAppEvents app)
    {
        app.Handles<TestEventFromHelperExtension, TestEventHandler>();
    }

    public static void IAppStructureHelperStaticMethod(IAppStructure app)
    {
        app.Uses<HelperStaticClassModule>();
    }

    public static void IAppStructureHelperExtension(this IAppStructure app)
    {
        app.Uses<HelperExtensionModule>();
    }
}
