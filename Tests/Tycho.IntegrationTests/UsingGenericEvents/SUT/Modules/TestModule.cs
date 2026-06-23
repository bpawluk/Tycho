using Microsoft.Extensions.DependencyInjection;
using Tycho.Events;
using Tycho.IntegrationTests.UsingGenericEvents.SUT.Modules.Handlers;
using Tycho.Modules;

namespace Tycho.IntegrationTests.UsingGenericEvents.SUT.Modules;

public record GenericModuleEvent<T>(T Data) : IEvent;

public record GenericModuleFinishedEvent<T>(T Data) : IEvent;

[TychoDefinition]
public class TestModule : TychoModule
{
    protected override void DefineContract(IModuleContract module) { }

    protected override void DefineEvents(IModuleEvents module)
    {
        module.Expects<GenericModuleEvent<int>>()
              .HandlesWith<GenericModuleEventHandler<int>>();

        module.Expects<GenericModuleEvent<string>>()
              .HandlesWith<GenericModuleEventHandler<string>>();

        module.Expects<GenericModuleFinishedEvent<int>>()
              .MapsTo<GenericAppForwardedEvent<int>>(payload => new(payload.Data))
              .Exposes();

        module.Expects<GenericModuleFinishedEvent<string>>()
              .MapsTo<GenericAppForwardedEvent<string>>(payload => new(payload.Data))
              .Exposes();
    }

    protected override void IncludeModules(IModuleStructure module) { }

    protected override void RegisterServices(IServiceCollection module) { }
}
