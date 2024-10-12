using Microsoft.Extensions.DependencyInjection;
using Tycho.IntegrationTests.RunningStartupLogic.SUT.Handlers;
using Tycho.IntegrationTests.RunningStartupLogic.SUT.Modules;
using Tycho.IntegrationTests.RunningStartupLogic.SUT.Services;
using TychoV2.Apps;
using TychoV2.Requests;

namespace Tycho.IntegrationTests.RunningStartupLogic.SUT;

// Handles
public record GetAppValueRequest : IRequest<string>;

public class TestApp : TychoApp
{
    protected override void DefineContract(IAppContract app)
    {
        app.Handles<GetAppValueRequest, string, GetValueRequestHandler>()
           .Forwards<GetModuleValueRequest, string, TestModule>();
    }

    protected override void IncludeModules(IAppStructure app)
    {
        app.Uses<TestModule>();
    }

    protected override void MapEvents(IAppEvents app) { }

    protected override void RegisterServices(IServiceCollection app)
    {
        app.AddSingleton<TestService>();
    }

    protected override Task Startup(IServiceProvider app)
    {
        app.GetRequiredService<TestService>().Value = "Test = Passed";
        return Task.CompletedTask;
    }
}
