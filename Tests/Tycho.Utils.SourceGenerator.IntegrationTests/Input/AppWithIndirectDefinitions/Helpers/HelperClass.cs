using Tycho.Apps;
using Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithIndirectDefinitions.Handlers;
using Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithIndirectDefinitions.Modules;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithIndirectDefinitions.Helpers;

internal class HelperClass
{
    public void DefineContract(IAppContract app)
    {
        app.Expects<TestRequestFromHelperClass, string>().HandlesWith<TestRequestHandler>();
    }

    public void DefineEvents(IAppEvents app)
    {
        app.Expects<TestEventFromHelperClass>().HandlesWith<TestEventHandler>();
    }

    public void IncludeModules(IAppStructure app)
    {
        app.Uses<HelperClassModule>();
    }
}
