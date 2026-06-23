using Microsoft.Extensions.DependencyInjection;
using Tycho.Apps;
using Tycho.IntegrationTests.UsingGenericRequests.SUT.Handlers;
using Tycho.IntegrationTests.UsingGenericRequests.SUT.Modules;
using Tycho.Requests;

namespace Tycho.IntegrationTests.UsingGenericRequests.SUT;

// Requests

public record GenericAppRequest<T>(T Data) : IRequest<GenericAppRequest<T>.Response<T>>
{
    public record Response<Q>(Q Data);
}

public record GenericAppRequestToForward<T>(T Data) : IRequest<GenericAppRequestToForward<T>.Response<T>>
{
    public record Response<Q>(Q Data);
}

[TychoDefinition]
public class TestApp : TychoApp
{
    protected override void DefineContract(IAppContract app)
    {
        app.Expects<GenericAppRequest<int>, GenericAppRequest<int>.Response<int>>()
           .HandlesWith<GenericAppRequestHandler<int>>();

        app.Expects<GenericAppRequest<string>, GenericAppRequest<string>.Response<string>>()
           .HandlesWith<GenericAppRequestHandler<string>>();

        app.Expects<GenericAppRequestToForward<int>, GenericAppRequestToForward<int>.Response<int>>()
           .MapsTo<GenericModuleRequest<int>, GenericModuleRequest<int>.Response<int>>(
               request => new GenericModuleRequest<int>(request.Data),
               response => new GenericAppRequestToForward<int>.Response<int>(response.Data))
           .ForwardsTo<TestModule>();

        app.Expects<GenericAppRequestToForward<string>, GenericAppRequestToForward<string>.Response<string>>()
           .MapsTo<GenericModuleRequest<string>, GenericModuleRequest<string>.Response<string>>(
               request => new GenericModuleRequest<string>(request.Data),
               response => new GenericAppRequestToForward<string>.Response<string>(response.Data))
           .ForwardsTo<TestModule>();
    }

    protected override void DefineEvents(IAppEvents app) { }

    protected override void IncludeModules(IAppStructure app)
    {
        app.Uses<TestModule>(contract =>
        {
            contract.Handle<GenericModuleRequiredRequest<int>, GenericModuleRequiredRequest<int>.Response<int>, GenericModuleRequiredRequestHandler<int>>();
            contract.Handle<GenericModuleRequiredRequest<string>, GenericModuleRequiredRequest<string>.Response<string>, GenericModuleRequiredRequestHandler<string>>();
        });
    }

    protected override void RegisterServices(IServiceCollection app) { }
}
