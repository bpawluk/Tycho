using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Tycho;
using Tycho.Contract.Inbox;
using Tycho.Contract.Outbox;
using Tycho.Messaging.Payload;
using Tycho.Structure;

namespace IntegrationTests.ForwardingMessages.SUT.Submodules;

// Incoming and Outgoing
internal record MappedBetaEvent(string Id, int preInterceptions, int postInterceptions) : IEvent
{
    public int PreInterceptions { get; set; } = preInterceptions;
    public int PostInterceptions { get; set; } = postInterceptions;
};

internal record MappedBetaRequest(string Id, int preInterceptions, int postInterceptions) : IRequest
{
    public int PreInterceptions { get; set; } = preInterceptions;
    public int PostInterceptions { get; set; } = postInterceptions;
};

internal record MappedBetaRequestWithResponse(string Id, int preInterceptions, int postInterceptions) : IRequest<BetaResponse>
{
    public int PreInterceptions { get; set; } = preInterceptions;
    public int PostInterceptions { get; set; } = postInterceptions;
};

internal record BetaResponse(string Content);

internal class BetaModule : TychoModule
{
    protected override void HandleIncomingMessages(IInboxDefinition module, IServiceProvider services)
    {
        var thisModule = services.GetRequiredService<IModule>();
        module.Events.Handle<EventToForward>(eventData => thisModule.Publish(eventData))
              .Events.Handle<MappedBetaEvent>(eventData => thisModule.Publish(eventData))
              .Requests.Handle<RequestToForward>(requestData => thisModule.Execute(requestData))
              .Requests.Handle<MappedBetaRequest>(requestData => thisModule.Execute(requestData))
              .Requests.Handle<RequestWithResponseToForward, string>(requestData => thisModule.Execute<RequestWithResponseToForward, string>(requestData))
              .Requests.Handle<MappedBetaRequestWithResponse, BetaResponse>(requestData => thisModule.Execute<MappedBetaRequestWithResponse, BetaResponse>(requestData));
    }

    protected override void DeclareOutgoingMessages(IOutboxDefinition module, IServiceProvider services)
    {
        module.Events.Declare<EventToForward>()
              .Events.Declare<MappedBetaEvent>()
              .Requests.Declare<RequestToForward>()
              .Requests.Declare<MappedBetaRequest>()
              .Requests.Declare<RequestWithResponseToForward, string>()
              .Requests.Declare<MappedBetaRequestWithResponse, BetaResponse>();
    }

    protected override void IncludeSubmodules(ISubstructureDefinition module, IServiceProvider services) { }

    protected override void RegisterServices(IServiceCollection services, IConfiguration configuration) { }
}
