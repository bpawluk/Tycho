using Microsoft.Extensions.DependencyInjection;
using Tycho.Apps;
using Tycho.IntegrationTests.RunningStartupLogic.SUT.Handlers;
using Tycho.IntegrationTests.RunningStartupLogic.SUT.Modules;
using Tycho.IntegrationTests.RunningStartupLogic.SUT.Services;
using Tycho.Requests;

namespace Tycho.IntegrationTests.RunningStartupLogic.SUT;

// Handles
public record GetAppValueRequest : IRequest<string>;

[TychoDefinition]
public class TestApp : TychoApp
{
    protected override void DefineContract(IAppContract app)
    {
        app.Expects<GetAppValueRequest, string>()
           .HandlesWith<GetValueRequestHandler>();

        app.Expects<GetModuleValueRequest, string>()
           .ForwardsTo<TestModule>();
    }

    protected override void DefineEvents(IAppEvents app) { }

    protected override void IncludeModules(IAppStructure app)
    {
        app.Uses<TestModule>();
    }

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
