using Microsoft.Extensions.DependencyInjection;
using Tycho.IntegrationTests.ProvidingSettings.SUT.Handlers;
using Tycho.IntegrationTests.ProvidingSettings.SUT.Settings;
using Tycho.Modules;
using Tycho.Requests;

namespace Tycho.IntegrationTests.ProvidingSettings.SUT.Modules;

// Handles
public record GetBetaValueRequest : IRequest<string>;

[TychoDefinition]
public partial class BetaModule : TychoModule
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Handles<GetBetaValueRequest, string, GetValueRequestHandler>();
    }

    protected override void DefineEvents(IModuleEvents module) { }

    protected override void IncludeModules(IModuleStructure module) { }

    protected override void RegisterServices(IServiceCollection module)
    {
        ModuleSettings moduleSettings = GetSettings<ModuleSettings>();
        module.AddSingleton(moduleSettings);

        OtherSettings otherSettings = GetSettings<OtherSettings>();
        module.AddSingleton(otherSettings);
    }
}
