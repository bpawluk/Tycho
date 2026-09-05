using Microsoft.Extensions.DependencyInjection;
using Tycho.IntegrationTests.UsingGenericRequests.SUT.Modules.Handlers;
using Tycho.Modules;
using Tycho.Requests;

namespace Tycho.IntegrationTests.UsingGenericRequests.SUT.Modules;

// Requests

public record GenericModuleRequest<T>(T Data) : IRequest<GenericModuleRequest<T>.Response<T>>
{
    public record Response<Q>(Q Data);
}

public record GenericModuleRequiredRequest<T>(T Data) : IRequest<GenericModuleRequiredRequest<T>.Response<T>>
{
    public record Response<Q>(Q Data);
}

[TychoDefinition]
public class TestModule : TychoModule
{
    protected override void DefineContract(IModuleContract module)
    {
        module.Requires<GenericModuleRequiredRequest<int>, GenericModuleRequiredRequest<int>.Response<int>>();
        module.Requires<GenericModuleRequiredRequest<string>, GenericModuleRequiredRequest<string>.Response<string>>();

        module.Expects<GenericModuleRequest<int>, GenericModuleRequest<int>.Response<int>>()
              .HandlesWith<GenericModuleRequestHandler<int>>();

        module.Expects<GenericModuleRequest<string>, GenericModuleRequest<string>.Response<string>>()
              .HandlesWith<GenericModuleRequestHandler<string>>();

    }

    protected override void DefineEvents(IModuleEvents module) { }

    protected override void IncludeModules(IModuleStructure module) { }

    protected override void RegisterServices(IServiceCollection module) { }
}
