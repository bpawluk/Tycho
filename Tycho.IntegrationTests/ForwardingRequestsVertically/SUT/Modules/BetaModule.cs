﻿using Microsoft.Extensions.DependencyInjection;
using TychoV2.Modules;

namespace Tycho.IntegrationTests.ForwardingRequestsVertically.SUT.Modules;

internal class BetaModule : TychoModule
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Forwards<Request, GammaModule>()
              .Forwards<RequestWithResponse, string, GammaModule>();

        module.Requires<Request>()
              .Requires<RequestWithResponse, string>();
    }

    protected override void IncludeModules(IModuleStructure module)
    {
        module.AddSubmodule<GammaModule>(contract =>
        {
            contract.Expose<Request>()
                    .Expose<RequestWithResponse, string>();
        });
    }

    protected override void MapEvents(IModuleEvents module) { }

    protected override void RegisterServices(IServiceCollection module) { }
}
