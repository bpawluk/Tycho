using Microsoft.Extensions.DependencyInjection;
using Tycho.Apps;
using Tycho.IntegrationTests.InterceptingRequests.SUT.Handlers;
using Tycho.IntegrationTests.InterceptingRequests.SUT.Interceptors;
using Tycho.IntegrationTests.InterceptingRequests.SUT.Modules.Alpha;
using Tycho.IntegrationTests.InterceptingRequests.SUT.Utils;
using Tycho.Requests;

namespace Tycho.IntegrationTests.InterceptingRequests.SUT;

public sealed record RequestToIntercept(List<string> Trace) : IRequest, ITraceableRequest;

public sealed record RequestWithResponseToIntercept(List<string> Trace) : IRequest<string>, ITraceableRequest;

[TychoDefinition]
public class TestApp : TychoApp
{
    protected override void DefineContract(IAppContract app)
    {
        app.Expects<RequestToIntercept>()
           .ForwardsTo<AlphaModule>();

        app.Expects<RequestWithResponseToIntercept, string>()
           .ForwardsTo<AlphaModule>();
    }

    protected override void DefineEvents(IAppEvents app) { }

    protected override void IncludeModules(IAppStructure app)
    {
        app.Uses<AlphaModule>(app =>
        {
            app.Fulfills<RequestToIntercept>()
               .HandlesWith<RequestToInterceptHandler>();

            app.Fulfills<RequestWithResponseToIntercept, string>()
               .HandlesWith<RequestWithResponseToInterceptHandler>();
        });
    }

    protected override void RegisterServices(IServiceCollection app)
    {
        app.AddRequestInterceptor(typeof(AppInterceptor<,>));
    }
}
