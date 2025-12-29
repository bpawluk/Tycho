using Microsoft.Extensions.DependencyInjection;
using Tycho.Apps;
using Tycho.IntegrationTests.SettingUpForHostApps.SUT.Handlers;
using Tycho.Requests;

namespace Tycho.IntegrationTests.SettingUpForHostApps.SUT;

// Handles
public record TestRequest : IRequest<string>;

public class TestApp : TychoApp
{
    protected override void DefineContract(IAppContract app)
    {
        app.Handles<TestRequest, string, TestRequestHandler>();
    }

    protected override void DefineEvents(IAppEvents app) { }

    protected override void IncludeModules(IAppStructure app) { }

    protected override void RegisterServices(IServiceCollection app) 
    {
        app.AddSingleton(Configuration);
    }
}