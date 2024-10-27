using Microsoft.Extensions.DependencyInjection;
using Tycho.Modules;
using Tycho.Persistence.EFCore.IntegrationTests.SUT.Handlers;

namespace Tycho.Persistence.EFCore.IntegrationTests.SUT;

internal class TestModule : TychoModule
{
    protected override void DefineContract(IModuleContract module) { }

    protected override void IncludeModules(IModuleStructure module) { }

    protected override void MapEvents(IModuleEvents module)
    {
        module.Handles<TestEvent, TestEventHandler>();
    }

    protected override void RegisterServices(IServiceCollection module) { }
}