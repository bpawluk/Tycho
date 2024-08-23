using IntegrationTests.ForwardingMessagesVertically.SUT.Submodules.Alpha.Interceptors;
using IntegrationTests.ForwardingMessagesVertically.SUT.Submodules.Beta;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Tycho;
using Tycho.Contract.Inbox;
using Tycho.Contract.Outbox;
using Tycho.Structure;

namespace IntegrationTests.ForwardingMessagesVertically.SUT.Submodules.Alpha;

internal class AlphaModule : TychoModule
{
    protected override void DeclareOutgoingMessages(IOutboxDefinition outbox, IServiceProvider services)
    {
        outbox.Events.Declare<EventToForward>()
              .Events.Declare<AlphaOutEvent>();

        outbox.Requests.Declare<RequestToForward>()
              .Requests.Declare<AlphaOutRequest>();

        outbox.Requests.Declare<RequestWithResponseToForward, string>()
              .Requests.Declare<AlphaOutRequestWithResponse, AlphaOutRequestWithResponse.Response>();
    }

    protected override void HandleIncomingMessages(IInboxDefinition inbox, IServiceProvider services)
    {
        inbox.Events.ForwardWithInterception<EventToForward, EventInterceptor, BetaModule>()
             .Events.ForwardWithInterception<AlphaInEvent, BetaInEvent, EventInterceptor, BetaModule>(
                eventData => new(eventData.Result));

        inbox.Requests.ForwardWithInterception<RequestToForward, RequestInterceptor, BetaModule>()
             .Requests.ForwardWithInterception<AlphaInRequest, BetaInRequest, RequestInterceptor, BetaModule>(
                requestData => new(requestData.Result));

        inbox.Requests.ForwardWithInterception<RequestWithResponseToForward, string, RequestInterceptor, BetaModule>()
             .Requests.ForwardWithInterception<
                AlphaInRequestWithResponse, AlphaInRequestWithResponse.Response,
                BetaInRequestWithResponse, BetaInRequestWithResponse.Response,
                RequestInterceptor,
                BetaModule>(
                    requestData => new(requestData.Result),
                    response => new(response.Value));
    }

    protected override void IncludeSubmodules(ISubstructureDefinition structure, IServiceProvider services) 
    {
        structure.AddSubmodule<BetaModule>(betaOutbox =>
        {
            betaOutbox.Events.ExposeWithInterception<EventToForward, EventInterceptor>()
                      .Events.ExposeWithInterception<BetaOutEvent, AlphaOutEvent, EventInterceptor>(eventData => new(eventData.Result));

            betaOutbox.Requests.ExposeWithInterception<RequestToForward, RequestInterceptor>()
                      .Requests.ExposeWithInterception<BetaOutRequest, AlphaOutRequest, RequestInterceptor>(requestData => new(requestData.Result));

            betaOutbox.Requests.ExposeWithInterception<RequestWithResponseToForward, string, RequestInterceptor>()
                      .Requests.ExposeWithInterception<
                          BetaOutRequestWithResponse, BetaOutRequestWithResponse.Response, 
                          AlphaOutRequestWithResponse, AlphaOutRequestWithResponse.Response,
                          RequestInterceptor>(
                              requestData => new(requestData.Result),
                              response => new(response.Value));
        });
    }

    protected override void RegisterServices(IServiceCollection services, IConfiguration configuration) { }
}
