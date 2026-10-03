using Microsoft.Extensions.DependencyInjection;
using Tycho.Apps;
using Tycho.IntegrationTests.RequiredRequestRouting.SUT.Modules;
using Tycho.IntegrationTests.RequiredRequestRouting.SUT.Modules.Settings;

namespace Tycho.IntegrationTests.RequiredRequestRouting.SUT;

[TychoDefinition]
public class TestApp(TestResult result) : TychoApp
{
    protected override void DefineContract(IAppContract app)
    {
        app.Expects<PlainCommand>().ForwardsTo<ParentModule>();
        app.Expects<PlainQuery, string>().ForwardsTo<ParentModule>();
        app.Expects<MappedCommand>().ForwardsTo<ParentModule>();
        app.Expects<MappedQuery, string>().ForwardsTo<ParentModule>();
        app.Expects<IgnoredCommand>().ForwardsTo<ParentModule>();
        app.Expects<IgnoredQuery, string>().ForwardsTo<ParentModule>();
        app.Expects<DirectIgnoredCommand>()
            .MapsTo<IgnoredCommand>(request => new(request.Value)).ForwardsTo<SourceModule>();
        app.Expects<DirectIgnoredQuery, string>()
            .MapsTo<IgnoredQuery, string>(request => new(request.Value), response => response)
            .ForwardsTo<SourceModule>();
    }

    protected override void DefineEvents(IAppEvents app) { }

    protected override void IncludeModules(IAppStructure app)
    {
        app.Uses<ParentModule>(new RoutingSettings { Result = result });
        app.Uses<SourceModule>(contract =>
        {
            contract.Fulfills<PlainCommand>().Ignores();
            contract.Fulfills<PlainQuery, string>().Ignores();
            contract.Fulfills<MappedCommand>().Ignores();
            contract.Fulfills<MappedQuery, string>().Ignores();
            contract.Fulfills<IgnoredCommand>().Ignores();
            contract.Fulfills<IgnoredQuery, string>().Ignores();
        });
    }

    protected override void RegisterServices(IServiceCollection app) { }
}
