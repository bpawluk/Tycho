using Microsoft.Extensions.DependencyInjection;
using Tycho.IntegrationTests.RequiredRequestRouting.SUT.Modules.Settings;
using Tycho.Modules;

namespace Tycho.IntegrationTests.RequiredRequestRouting.SUT.Modules;

[TychoDefinition]
public class ParentModule : TychoModule
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Expects<PlainCommand>().ForwardsTo<SourceModule>();
        module.Expects<PlainQuery, string>().ForwardsTo<SourceModule>();
        module.Expects<MappedCommand>().ForwardsTo<SourceModule>();
        module.Expects<MappedQuery, string>().ForwardsTo<SourceModule>();
        module.Expects<IgnoredCommand>().ForwardsTo<SourceModule>();
        module.Expects<IgnoredQuery, string>().ForwardsTo<SourceModule>();
    }

    protected override void DefineEvents(IModuleEvents module) { }

    protected override void IncludeModules(IModuleStructure module)
    {
        module.Uses<SourceModule>(contract =>
        {
            contract.Fulfills<PlainCommand>().ForwardsTo<DestinationModule>();
            contract.Fulfills<PlainQuery, string>().ForwardsTo<DestinationModule>();
            contract.Fulfills<MappedCommand>()
                .MapsTo<TargetCommand>(request => new("mapped:" + request.Value))
                .ForwardsTo<DestinationModule>();
            contract.Fulfills<MappedQuery, string>()
                .MapsTo<TargetQuery, int>(request => new("mapped:" + request.Value), response => "mapped-response:" + response)
                .ForwardsTo<DestinationModule>();
            contract.Fulfills<IgnoredCommand>().Ignores();
            contract.Fulfills<IgnoredQuery, string>().Ignores();
        }, instanceSuffix: "in-parent-module");
        module.Uses<DestinationModule>(GetSettings<RoutingSettings>());
        module.Uses<UnrelatedModule>(GetSettings<RoutingSettings>());
    }

    protected override void RegisterServices(IServiceCollection module) { }
}
