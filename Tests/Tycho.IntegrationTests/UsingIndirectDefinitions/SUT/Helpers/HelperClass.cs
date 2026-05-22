using Tycho.Apps;
using Tycho.IntegrationTests.UsingIndirectDefinitions.SUT.Handlers;
using Tycho.IntegrationTests.UsingIndirectDefinitions.SUT.Modules;

namespace Tycho.IntegrationTests.UsingIndirectDefinitions.SUT.Helpers;

internal class HelperClass
{

#pragma warning disable CA1822

    public void IAppContractHelperMethod(IAppContract app)
    {
        app.Handles<TestRequestFromHelperClass, string, TestRequestHandler>();
    }

    public void IAppEventsHelperMethod(IAppEvents app)
    {
        app.Handles<TestEventFromHelperClass, TestEventHandler>();
    }

    public void IAppStructureHelperMethod(IAppStructure app)
    {
        app.Uses<HelperClassModule>();
    }

#pragma warning restore CA1822

}
