using IntegrationTests.ForwardingMessages.SUT.Interceptors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Tycho;
using Tycho.Contract.Inbox;
using Tycho.Contract.Outbox;
using Tycho.Messaging.Payload;
using Tycho.Structure;

namespace IntegrationTests.ForwardingMessages.SUT.Submodules;

// Incoming
internal record MappedAlphaEvent(string Id, int preInterceptions, int postInterceptions) : IEvent
{
    public int PreInterceptions { get; set; } = preInterceptions;
    public int PostInterceptions { get; set; } = postInterceptions;
};

internal record MappedAlphaRequest(string Id, int preInterceptions, int postInterceptions) : IRequest
{
    public int PreInterceptions { get; set; } = preInterceptions;
    public int PostInterceptions { get; set; } = postInterceptions;
};

internal record MappedAlphaRequestWithResponse(string Id, int preInterceptions, int postInterceptions) : IRequest<AlphaResponse>
{
    public int PreInterceptions { get; set; } = preInterceptions;
    public int PostInterceptions { get; set; } = postInterceptions;
};

// Outgoing
// no outgoing messages

internal record AlphaResponse(string Content);

internal class AlphaModule : TychoModule
{
    protected override void HandleIncomingMessages(IInboxDefinition module, IServiceProvider services)
    {
        module.Events.ForwardWithInterception<EventToForward, EventInterceptor, BetaModule>()
              .Events.ForwardWithInterception<MappedAlphaEvent, MappedBetaEvent, EventInterceptor, BetaModule>(
                  eventData => new(eventData.Id, eventData.PreInterceptions, eventData.PostInterceptions))
              .Requests.ForwardWithInterception<RequestToForward, RequestInterceptor, BetaModule>()
              .Requests.ForwardWithInterception<MappedAlphaRequest, MappedBetaRequest, RequestInterceptor, BetaModule>(
                  requestData => new(requestData.Id, requestData.PreInterceptions, requestData.PostInterceptions))
              .Requests.ForwardWithInterception<RequestWithResponseToForward, string, RequestWithResponseInterceptor, BetaModule>()
              .Requests.ForwardWithInterception<MappedAlphaRequestWithResponse, AlphaResponse, MappedBetaRequestWithResponse, BetaResponse, RequestWithResponseInterceptor, BetaModule>(
                  requestData => new(requestData.Id, requestData.PreInterceptions, requestData.PostInterceptions), 
                  result => new(result.Content));
    }

    protected override void DeclareOutgoingMessages(IOutboxDefinition module, IServiceProvider services) 
    {
        module.Events.Declare<EventToForward>()
              .Events.Declare<MappedAlphaEvent>()
              .Requests.Declare<RequestToForward>()
              .Requests.Declare<MappedAlphaRequest>()
              .Requests.Declare<RequestWithResponseToForward, string>()
              .Requests.Declare<MappedAlphaRequestWithResponse, AlphaResponse>();
    }

    protected override void IncludeSubmodules(ISubstructureDefinition module, IServiceProvider services) 
    {
        module.AddSubmodule<BetaModule>(consumer =>
        {
            consumer.Events.ExposeWithInterception<EventToForward, EventInterceptor>()
                    .Events.ExposeWithInterception<MappedBetaEvent, MappedAlphaEvent, EventInterceptor>(
                        eventData => new(eventData.Id, eventData.PreInterceptions, eventData.PostInterceptions))
                    .Requests.ExposeWithInterception<RequestToForward, RequestInterceptor>()
                    .Requests.ExposeWithInterception<MappedBetaRequest, MappedAlphaRequest, RequestInterceptor>(
                        requestData => new(requestData.Id, requestData.PreInterceptions, requestData.PostInterceptions))
                    .Requests.ExposeWithInterception<RequestWithResponseToForward, string, RequestWithResponseInterceptor>()
                    .Requests.ExposeWithInterception<MappedBetaRequestWithResponse, BetaResponse, MappedAlphaRequestWithResponse, AlphaResponse, RequestWithResponseInterceptor>(
                        requestData => new(requestData.Id, requestData.PreInterceptions, requestData.PostInterceptions), 
                        result => new(result.Content));
        });
    }

    protected override void RegisterServices(IServiceCollection services, IConfiguration configuration) { }
}
