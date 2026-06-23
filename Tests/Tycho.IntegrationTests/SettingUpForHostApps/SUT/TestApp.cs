using Microsoft.Extensions.DependencyInjection;
using Tycho.Apps;
using Tycho.IntegrationTests.SettingUpForHostApps.SUT.Handlers;
using Tycho.Requests;

namespace Tycho.IntegrationTests.SettingUpForHostApps.SUT;

// Handles
public record TestRequest : IRequest<string>;

[TychoDefinition]
public class TestApp : TychoApp
{
    protected override void DefineContract(IAppContract app)
    {
        app.Expects<TestRequest, string>()
           .HandlesWith<TestRequestHandler>();
    }

    protected override void DefineEvents(IAppEvents app) { }

    protected override void IncludeModules(IAppStructure app) { }

    protected override void RegisterServices(IServiceCollection app)
    {
        app.AddSingleton(Configuration);
    }
}
