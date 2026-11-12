using Microsoft.Extensions.DependencyInjection;
using Tycho.IntegrationTests.RequiredRequestRouting.SUT.Modules.Handlers;
using Tycho.IntegrationTests.RequiredRequestRouting.SUT.Modules.Settings;
using Tycho.Modules;

namespace Tycho.IntegrationTests.RequiredRequestRouting.SUT.Modules;

[TychoDefinition]
public class DestinationModule : TychoModule
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Expects<PlainCommand>().HandlesWith<DestinationRequestHandler>();
        module.Expects<PlainQuery, string>().HandlesWith<DestinationRequestHandler>();
        module.Expects<TargetCommand>().HandlesWith<DestinationRequestHandler>();
        module.Expects<TargetQuery, int>().HandlesWith<DestinationRequestHandler>();
        module.Expects<IgnoredCommand>().HandlesWith<DestinationRequestHandler>();
        module.Expects<IgnoredQuery, string>().HandlesWith<DestinationRequestHandler>();
    }

    protected override void DefineEvents(IModuleEvents module) { }
    protected override void IncludeModules(IModuleStructure module) { }
    protected override void RegisterServices(IServiceCollection module)
    {
        module.AddSingleton(GetSettings<RoutingSettings>().Result);
        module.AddSingleton(new DestinationName("destination"));
    }
}
