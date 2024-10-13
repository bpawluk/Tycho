using Microsoft.Extensions.DependencyInjection;
using TychoV2.Modules;

namespace Tycho.IntegrationTests.ForwardingRequestsVertically.SUT.Modules;

internal class AlphaModule : TychoModule
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Forwards<Request, BetaModule>()
              .Forwards<RequestWithResponse, string, BetaModule>();

        module.Requires<Request>()
              .Requires<RequestWithResponse, string>();
    }

    protected override void IncludeModules(IModuleStructure module)
    {
        module.Uses<BetaModule>(contract =>
        {
            contract.Expose<Request>()
                    .Expose<RequestWithResponse, string>();
        });
    }

    protected override void MapEvents(IModuleEvents module) { }

    protected override void RegisterServices(IServiceCollection module) { }
}
