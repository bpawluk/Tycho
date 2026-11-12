using Microsoft.Extensions.DependencyInjection;
using Tycho.IntegrationTests.SendingRequestsHorizontally.SUT.Modules.Alpha.Handlers;
using Tycho.Modules;
using Tycho.Requests;

namespace Tycho.IntegrationTests.SendingRequestsHorizontally.SUT.Modules.Alpha;

// Handles
public record AlphaInRequest(TestResult Result) : IRequest;
public record AlphaInRequestWithResponse(TestResult Result) : IRequest<string>;

// Requires
public record AlphaOutRequest(TestResult Result) : IRequest;
public record AlphaOutRequestWithResponse(TestResult Result) : IRequest<string>;

[TychoDefinition]
public class AlphaModule : TychoModule
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Requires<AlphaOutRequest>();
        module.Requires<AlphaOutRequestWithResponse, string>();

        module.Expects<AlphaInRequest>()
              .HandlesWith<AlphaInRequestHandler>();

        module.Expects<AlphaInRequestWithResponse, string>()
              .HandlesWith<AlphaInRequestHandler>();
    }

    protected override void DefineEvents(IModuleEvents module) { }

    protected override void IncludeModules(IModuleStructure module) { }

    protected override void RegisterServices(IServiceCollection module) { }
}
