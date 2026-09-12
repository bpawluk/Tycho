using Microsoft.Extensions.DependencyInjection;
using Tycho.IntegrationTests.InterceptingRequests.SUT.Modules.Alpha.Handlers;
using Tycho.IntegrationTests.InterceptingRequests.SUT.Modules.Alpha.Interceptors;
using Tycho.Modules;
using Tycho.Requests;

namespace Tycho.IntegrationTests.InterceptingRequests.SUT.Modules.Alpha;

[TychoDefinition]
public class AlphaModule : TychoModule
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Requires<RequestToIntercept>();
        module.Requires<RequestWithResponseToIntercept, string>();

        module.Expects<RequestToIntercept>()
              .HandlesWith<RequestToInterceptHandler>();

        module.Expects<RequestWithResponseToIntercept, string>()
              .HandlesWith<RequestWithResponseToInterceptHandler>();
    }

    protected override void DefineEvents(IModuleEvents module) { }

    protected override void IncludeModules(IModuleStructure module) { }

    protected override void RegisterServices(IServiceCollection module)
    {
        module.AddRequestInterceptor(typeof(ModuleInterceptor<,>));
    }
}

