using Microsoft.Extensions.DependencyInjection;
using Tycho.IntegrationTests.RunningStartupLogic.SUT.Handlers;
using Tycho.IntegrationTests.RunningStartupLogic.SUT.Services;
using Tycho.Modules;
using Tycho.Requests;

namespace Tycho.IntegrationTests.RunningStartupLogic.SUT.Modules;

// Handles
public record GetModuleValueRequest : IRequest<string>;

internal class TestModule : TychoModule
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Handles<GetModuleValueRequest, string, GetValueRequestHandler>();
    }

    protected override void IncludeModules(IModuleStructure module) { }

    protected override void MapEvents(IModuleEvents module) { }

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