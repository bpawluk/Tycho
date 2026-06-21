using Microsoft.Extensions.DependencyInjection;
using Tycho.IntegrationTests.ProvidingSettings.SUT.Handlers;
using Tycho.IntegrationTests.ProvidingSettings.SUT.Settings;
using Tycho.Modules;
using Tycho.Requests;

namespace Tycho.IntegrationTests.ProvidingSettings.SUT.Modules;

// Handles
public record GetAlphaValueRequest : IRequest<string>;

[TychoDefinition]
public class AlphaModule : TychoModule
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Forwards<GetBetaValueRequest, string, BetaModule>()
              .Handles<GetAlphaValueRequest, string, GetValueRequestHandler>();
    }

    protected override void DefineEvents(IModuleEvents module) { }

    protected override void IncludeModules(IModuleStructure module)
    {
        ModuleSettings moduleSettings = GetSettings<ModuleSettings>();
        module.Uses<BetaModule>(moduleSettings);
    }

    protected override void RegisterServices(IServiceCollection module)
    {
        ModuleSettings moduleSettings = GetSettings<ModuleSettings>();
        module.AddSingleton(moduleSettings);

        OtherSettings otherSettings = GetSettings<OtherSettings>();
        module.AddSingleton(otherSettings);
    }
}
