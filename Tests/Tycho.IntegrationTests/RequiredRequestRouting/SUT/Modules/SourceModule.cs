using Microsoft.Extensions.DependencyInjection;
using Tycho.IntegrationTests.RequiredRequestRouting.SUT.Modules.Handlers;
using Tycho.Modules;

namespace Tycho.IntegrationTests.RequiredRequestRouting.SUT.Modules;

[TychoDefinition]
public class SourceModule : TychoModule
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Requires<PlainCommand>();
        module.Requires<PlainQuery, string>();
        module.Requires<MappedCommand>();
        module.Requires<MappedQuery, string>();
        module.Requires<IgnoredCommand>();
        module.Requires<IgnoredQuery, string>();
        module.Expects<PlainCommand>().HandlesWith<SourceRequestHandler>();
        module.Expects<PlainQuery, string>().HandlesWith<SourceRequestHandler>();
        module.Expects<MappedCommand>().HandlesWith<SourceRequestHandler>();
        module.Expects<MappedQuery, string>().HandlesWith<SourceRequestHandler>();
        module.Expects<IgnoredCommand>().HandlesWith<SourceRequestHandler>();
        module.Expects<IgnoredQuery, string>().HandlesWith<SourceRequestHandler>();
    }

    protected override void DefineEvents(IModuleEvents module) { }
    protected override void IncludeModules(IModuleStructure module) { }
    protected override void RegisterServices(IServiceCollection module) { }
}
