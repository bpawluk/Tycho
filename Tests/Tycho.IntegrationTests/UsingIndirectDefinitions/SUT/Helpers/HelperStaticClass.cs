using Tycho.Apps;
using Tycho.IntegrationTests.UsingIndirectDefinitions.SUT.Handlers;

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
}
