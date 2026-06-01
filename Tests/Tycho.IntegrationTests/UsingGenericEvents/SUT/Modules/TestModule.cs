using Microsoft.Extensions.DependencyInjection;
using Tycho.Events;
using Tycho.IntegrationTests.UsingGenericEvents.SUT.Modules.Handlers;
using Tycho.Modules;

namespace Tycho.IntegrationTests.UsingGenericEvents.SUT.Modules;

public record GenericModuleEvent<T>(T Data) : IEvent;

public record GenericModuleFinishedEvent<T>(T Data) : IEvent;

[TychoDefinition]
public partial class TestModule : TychoModule
{
    protected override void DefineContract(IModuleContract module) { }

    protected override void DefineEvents(IModuleEvents module)
    {
        module.Handles<GenericModuleEvent<int>, GenericModuleEventHandler<int>>();
        module.Handles<GenericModuleEvent<string>, GenericModuleEventHandler<string>>();

        module.Routes<GenericModuleFinishedEvent<int>>()
              .ExposesAs(eventData => new GenericAppForwardedEvent<int>(eventData.Data));

        module.Routes<GenericModuleFinishedEvent<string>>()
              .ExposesAs(eventData => new GenericAppForwardedEvent<string>(eventData.Data));
    }

    protected override void IncludeModules(IModuleStructure module) { }

    protected override void RegisterServices(IServiceCollection module) { }
}
