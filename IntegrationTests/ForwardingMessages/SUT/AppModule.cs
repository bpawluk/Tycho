using IntegrationTests.ForwardingMessages.SUT.Interceptors;
using IntegrationTests.ForwardingMessages.SUT.Submodules;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Tycho;
using Tycho.Contract.Inbox;
using Tycho.Contract.Outbox;
using Tycho.Messaging.Payload;
using Tycho.Structure;

namespace IntegrationTests.ForwardingMessages.SUT;

// Incoming
internal record EventToForward(string Id, int preInterceptions, int postInterceptions) : IEvent
{
    public int PreInterceptions { get; set; } = preInterceptions;
    public int PostInterceptions { get; set; } = postInterceptions;
};

internal record EventToForwardWithMapping(string Id, int preInterceptions, int postInterceptions) : IEvent
{
    public int PreInterceptions { get; set; } = preInterceptions;
    public int PostInterceptions { get; set; } = postInterceptions;
};

internal record RequestToForward(string Id, int preInterceptions, int postInterceptions) : IRequest
{
    public int PreInterceptions { get; set; } = preInterceptions;
    public int PostInterceptions { get; set; } = postInterceptions;
};

internal record RequestToForwardWithMapping(string Id, int preInterceptions, int postInterceptions) : IRequest
{
    public int PreInterceptions { get; set; } = preInterceptions;
    public int PostInterceptions { get; set; } = postInterceptions;
};

internal record RequestWithResponseToForward(string Id, int preInterceptions, int postInterceptions) : IRequest<string>
{
    public int PreInterceptions { get; set; } = preInterceptions;
    public int PostInterceptions { get; set; } = postInterceptions;
};

internal record RequestWithResponseToForwardWithMapping(string Id, int preInterceptions, int postInterceptions) : IRequest<string>
{
    public int PreInterceptions { get; set; } = preInterceptions;
    public int PostInterceptions { get; set; } = postInterceptions;
};

// Outgoing
internal record MappedEvent(string Id, int preInterceptions, int postInterceptions) : IEvent
{
    public int PreInterceptions { get; set; } = preInterceptions;
    public int PostInterceptions { get; set; } = postInterceptions;
};

internal record MappedRequest(string Id, int preInterceptions, int postInterceptions) : IRequest
{
    public int PreInterceptions { get; set; } = preInterceptions;
    public int PostInterceptions { get; set; } = postInterceptions;
};

internal record MappedRequestWithResponse(string Id, int preInterceptions, int postInterceptions) : IRequest<string>
{
    public int PreInterceptions { get; set; } = preInterceptions;
    public int PostInterceptions { get; set; } = postInterceptions;
};

internal class AppModule : TychoModule
{
    protected override void HandleIncomingMessages(IInboxDefinition module, IServiceProvider services)
    {
        module.Events.ForwardWithInterception<EventToForward, EventInterceptor, AlphaModule>()
              .Events.ForwardWithInterception<EventToForwardWithMapping, MappedAlphaEvent, EventInterceptor, AlphaModule>(
                  eventData => new(eventData.Id, eventData.PreInterceptions, eventData.PostInterceptions))
              .Requests.ForwardWithInterception<RequestToForward, RequestInterceptor, AlphaModule>()
              .Requests.ForwardWithInterception<RequestToForwardWithMapping, MappedAlphaRequest, RequestInterceptor, AlphaModule>(
                  requestData => new(requestData.Id, requestData.PreInterceptions, requestData.PostInterceptions))
              .Requests.ForwardWithInterception<RequestWithResponseToForward, string, RequestWithResponseInterceptor, AlphaModule>()
              .Requests.ForwardWithInterception<RequestWithResponseToForwardWithMapping, string, MappedAlphaRequestWithResponse, AlphaResponse, RequestWithResponseInterceptor, AlphaModule>(
                  requestData => new(requestData.Id, requestData.PreInterceptions, requestData.PostInterceptions),
                  response => response.Content);
    }

    protected override void DeclareOutgoingMessages(IOutboxDefinition module, IServiceProvider services)
    {
        module.Events.Declare<EventToForward>()
              .Events.Declare<MappedEvent>()
              .Requests.Declare<RequestToForward>()
              .Requests.Declare<MappedRequest>()
              .Requests.Declare<RequestWithResponseToForward, string>()
              .Requests.Declare<MappedRequestWithResponse, string>();
    }

    protected override void IncludeSubmodules(ISubstructureDefinition module, IServiceProvider services)
    {
        module.AddSubmodule<AlphaModule>(consumer =>
        {
            consumer.Events.ExposeWithInterception<EventToForward, EventInterceptor>()
                    .Events.ExposeWithInterception<MappedAlphaEvent, MappedEvent, EventInterceptor>(
                        eventData => new(eventData.Id, eventData.PreInterceptions, eventData.PostInterceptions))
                    .Requests.ExposeWithInterception<RequestToForward, RequestInterceptor>()
                    .Requests.ExposeWithInterception<MappedAlphaRequest, MappedRequest, RequestInterceptor>(
                        requestData => new(requestData.Id, requestData.PreInterceptions, requestData.PostInterceptions))
                    .Requests.ExposeWithInterception<RequestWithResponseToForward, string, RequestWithResponseInterceptor>()
                    .Requests.ExposeWithInterception<MappedAlphaRequestWithResponse, AlphaResponse, MappedRequestWithResponse, string, RequestWithResponseInterceptor>(
                        requestData => new(requestData.Id, requestData.PreInterceptions, requestData.PostInterceptions),
                        result => new(result));
        });
    }

    protected override void RegisterServices(IServiceCollection services, IConfiguration configuration) { }
}
