﻿using Microsoft.Extensions.DependencyInjection;
using Tycho.IntegrationTests.ForwardingRequestsHorizontally.SUT.Modules.Handlers;
using Tycho.Modules;

namespace Tycho.IntegrationTests.ForwardingRequestsHorizontally.SUT.Modules;

internal class AlphaModule : TychoModule
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Handles<Request, RequestHandler>()
              .Handles<RequestWithResponse, string, RequestHandler>();

        module.Requires<Request>()
              .Requires<RequestWithResponse, string>();
    }

    protected override void IncludeModules(IModuleStructure module) { }

    protected override void MapEvents(IModuleEvents module) { }

    protected override void RegisterServices(IServiceCollection module) { }
}
