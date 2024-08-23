using IntegrationTests.ForwardingMessagesHorizontally.SUT.Interceptors;
using IntegrationTests.ForwardingMessagesHorizontally.SUT.Submodules.Alpha;
using IntegrationTests.ForwardingMessagesHorizontally.SUT.Submodules.Beta;
using IntegrationTests.ForwardingMessagesHorizontally.SUT.Submodules.Gamma;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Tycho;
using Tycho.Contract.Inbox;
using Tycho.Contract.Outbox;
using Tycho.Structure;

namespace IntegrationTests.ForwardingMessagesHorizontally.SUT;

internal class AppModule : TychoModule
{
    protected override void DeclareOutgoingMessages(IOutboxDefinition outbox, IServiceProvider services)
    {
        outbox.Events.Declare<EventToForward>()
              .Events.Declare<MappedEvent>();

        outbox.Requests.Declare<RequestToForward>()
              .Requests.Declare<MappedRequest>();

        outbox.Requests.Declare<RequestWithResponseToForward, string>()
              .Requests.Declare<MappedRequestWithResponse, string>();
    }

    protected override void HandleIncomingMessages(IInboxDefinition inbox, IServiceProvider services)
    {
        inbox.Events.Forward<EventToForward, AlphaModule>()
             .Events.Forward<EventToForwardWithMapping, AlphaInEvent, AlphaModule>(
                eventData => new(eventData.Result));

        inbox.Requests.Forward<RequestToForward, AlphaModule>()
             .Requests.Forward<RequestToForwardWithMapping, AlphaInRequest, AlphaModule>(
                requestData => new(requestData.Result));

        inbox.Requests.Forward<RequestWithResponseToForward, string, AlphaModule>()
             .Requests.Forward<
                RequestWithResponseToForwardWithMapping, string, 
                AlphaInRequestWithResponse, AlphaInRequestWithResponse.Response, 
                AlphaModule>(
                    requestData => new(requestData.Result),
                    response => response.Value);
    }

    protected override void IncludeSubmodules(ISubstructureDefinition structure, IServiceProvider services)
    {
        structure.AddSubmodule<AlphaModule>(alphaOutbox =>
        {
            alphaOutbox.Events.ForwardWithInterception<EventToForward, EventInterceptor, BetaModule>()
                       .Events.ForwardWithInterception<AlphaOutEvent, BetaInEvent, EventInterceptor, BetaModule>(
                            eventData => new(eventData.Result));

            alphaOutbox.Requests.ForwardWithInterception<RequestToForward, RequestInterceptor, BetaModule>()
                       .Requests.ForwardWithInterception<AlphaOutRequest, BetaInRequest, RequestInterceptor, BetaModule>(
                            requestData => new(requestData.Result));

            alphaOutbox.Requests.ForwardWithInterception<RequestWithResponseToForward, string, RequestInterceptor, BetaModule>()
                       .Requests.ForwardWithInterception<
                            AlphaOutRequestWithResponse, AlphaOutRequestWithResponse.Response,
                            BetaInRequestWithResponse, BetaInRequestWithResponse.Response,
                            RequestInterceptor,
                            BetaModule>(
                                requestData => new(requestData.Result),
                                response => new(response.Value));
        });

        structure.AddSubmodule<BetaModule>(alphaOutbox =>
        {
            alphaOutbox.Events.ForwardWithInterception<EventToForward, EventInterceptor, GammaModule>()
                       .Events.ForwardWithInterception<BetaOutEvent, GammaInEvent, EventInterceptor, GammaModule>(
                            eventData => new(eventData.Result));

            alphaOutbox.Requests.ForwardWithInterception<RequestToForward, RequestInterceptor, GammaModule>()
                       .Requests.ForwardWithInterception<BetaOutRequest, GammaInRequest, RequestInterceptor, GammaModule>(
                            requestData => new(requestData.Result));

            alphaOutbox.Requests.ForwardWithInterception<RequestWithResponseToForward, string, RequestInterceptor, GammaModule>()
                       .Requests.ForwardWithInterception<
                            BetaOutRequestWithResponse, BetaOutRequestWithResponse.Response,
                            GammaInRequestWithResponse, GammaInRequestWithResponse.Response,
                            RequestInterceptor,
                            GammaModule>(
                                requestData => new(requestData.Result),
                                response => new(response.Value));
        });

        structure.AddSubmodule<GammaModule>(alphaOutbox =>
        {
            alphaOutbox.Events.Expose<EventToForward>()
                       .Events.Expose<GammaOutEvent, MappedEvent>(
                            eventData => new(eventData.Result));

            alphaOutbox.Requests.Expose<RequestToForward>()
                       .Requests.Expose<GammaOutRequest, MappedRequest>(
                            requestData => new(requestData.Result));

            alphaOutbox.Requests.Expose<RequestWithResponseToForward, string>()
                       .Requests.Expose<
                            GammaOutRequestWithResponse, GammaOutRequestWithResponse.Response,
                            MappedRequestWithResponse, string>(
                                requestData => new(requestData.Result),
                                response => new(response));
        });
    }

    protected override void RegisterServices(IServiceCollection services, IConfiguration configuration) { }
}
