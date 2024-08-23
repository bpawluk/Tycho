using IntegrationTests.ForwardingMessagesVertically.SUT.Submodules.Beta.Interceptors;
using IntegrationTests.ForwardingMessagesVertically.SUT.Submodules.Gamma;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Tycho;
using Tycho.Contract.Inbox;
using Tycho.Contract.Outbox;
using Tycho.Structure;

namespace IntegrationTests.ForwardingMessagesVertically.SUT.Submodules.Beta;

internal class BetaModule : TychoModule
{
    protected override void DeclareOutgoingMessages(IOutboxDefinition outbox, IServiceProvider services)
    {
        outbox.Events.Declare<EventToForward>()
              .Events.Declare<BetaOutEvent>();

        outbox.Requests.Declare<RequestToForward>()
              .Requests.Declare<BetaOutRequest>();

        outbox.Requests.Declare<RequestWithResponseToForward, string>()
              .Requests.Declare<BetaOutRequestWithResponse, BetaOutRequestWithResponse.Response>();
    }

    protected override void HandleIncomingMessages(IInboxDefinition inbox, IServiceProvider services)
    {
        inbox.Events.ForwardWithInterception<EventToForward, EventInterceptor, GammaModule>()
             .Events.ForwardWithInterception<BetaInEvent, GammaInEvent, EventInterceptor, GammaModule>(
                eventData => new(eventData.Result));

        inbox.Requests.ForwardWithInterception<RequestToForward, RequestInterceptor, GammaModule>()
             .Requests.ForwardWithInterception<BetaInRequest, GammaInRequest, RequestInterceptor, GammaModule>(
                requestData => new(requestData.Result));

        inbox.Requests.ForwardWithInterception<RequestWithResponseToForward, string, RequestInterceptor, GammaModule>()
             .Requests.ForwardWithInterception<
                BetaInRequestWithResponse, BetaInRequestWithResponse.Response,
                GammaInRequestWithResponse, GammaInRequestWithResponse.Response,
                RequestInterceptor,
                GammaModule>(
                    requestData => new(requestData.Result),
                    response => new(response.Value));
    }

    protected override void IncludeSubmodules(ISubstructureDefinition structure, IServiceProvider services) 
    {
        structure.AddSubmodule<GammaModule>(betaOutbox =>
        {
            betaOutbox.Events.ExposeWithInterception<EventToForward, EventInterceptor>()
                      .Events.ExposeWithInterception<GammaOutEvent, BetaOutEvent, EventInterceptor>(eventData => new(eventData.Result));

            betaOutbox.Requests.ExposeWithInterception<RequestToForward, RequestInterceptor>()
                      .Requests.ExposeWithInterception<GammaOutRequest, BetaOutRequest, RequestInterceptor>(requestData => new(requestData.Result));

            betaOutbox.Requests.ExposeWithInterception<RequestWithResponseToForward, string, RequestInterceptor>()
                      .Requests.ExposeWithInterception<
                          GammaOutRequestWithResponse, GammaOutRequestWithResponse.Response,
                          BetaOutRequestWithResponse, BetaOutRequestWithResponse.Response,
                          RequestInterceptor>(
                              requestData => new(requestData.Result),
                              response => new(response.Value));
        });
    }

    protected override void RegisterServices(IServiceCollection services, IConfiguration configuration) { }
}
