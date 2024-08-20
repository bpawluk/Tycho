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

internal record MappedBetaCommand(string Id, int preInterceptions, int postInterceptions) : IRequest
{
    public int PreInterceptions { get; set; } = preInterceptions;
    public int PostInterceptions { get; set; } = postInterceptions;
};

internal record MappedBetaQuery(string Id, int preInterceptions, int postInterceptions) : IRequest<BetaResponse>
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
        module.SubscribesTo<EventToForward>(eventData => thisModule.Publish(eventData))
              .SubscribesTo<MappedBetaEvent>(eventData => thisModule.Publish(eventData))
              .Executes<CommandToForward>(commandData => thisModule.Execute(commandData))
              .Executes<MappedBetaCommand>(commandData => thisModule.Execute(commandData))
              .RespondsTo<QueryToForward, string>(queryData => thisModule.Execute<QueryToForward, string>(queryData))
              .RespondsTo<MappedBetaQuery, BetaResponse>(queryData => thisModule.Execute<MappedBetaQuery, BetaResponse>(queryData));
    }

    protected override void DeclareOutgoingMessages(IOutboxDefinition module, IServiceProvider services)
    {
        module.Publishes<EventToForward>()
              .Publishes<MappedBetaEvent>()
              .Sends<CommandToForward>()
              .Sends<MappedBetaCommand>()
              .Sends<QueryToForward, string>()
              .Sends<MappedBetaQuery, BetaResponse>();
    }

    protected override void IncludeSubmodules(ISubstructureDefinition module, IServiceProvider services) { }

    protected override void RegisterServices(IServiceCollection services, IConfiguration configuration) { }
}
