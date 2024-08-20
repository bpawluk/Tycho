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

internal record MappedAlphaCommand(string Id, int preInterceptions, int postInterceptions) : IRequest
{
    public int PreInterceptions { get; set; } = preInterceptions;
    public int PostInterceptions { get; set; } = postInterceptions;
};

internal record MappedAlphaQuery(string Id, int preInterceptions, int postInterceptions) : IRequest<AlphaResponse>
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
              .Requests.ForwardWithInterception<CommandToForward, CommandInterceptor, BetaModule>()
              .Requests.ForwardWithInterception<MappedAlphaCommand, MappedBetaCommand, CommandInterceptor, BetaModule>(
                  commandData => new(commandData.Id, commandData.PreInterceptions, commandData.PostInterceptions))
              .Requests.ForwardWithInterception<QueryToForward, string, QueryInterceptor, BetaModule>()
              .Requests.ForwardWithInterception<MappedAlphaQuery, AlphaResponse, MappedBetaQuery, BetaResponse, QueryInterceptor, BetaModule>(
                  queryData => new(queryData.Id, queryData.PreInterceptions, queryData.PostInterceptions), 
                  result => new(result.Content));
    }

    protected override void DeclareOutgoingMessages(IOutboxDefinition module, IServiceProvider services) 
    {
        module.Events.Declare<EventToForward>()
              .Events.Declare<MappedAlphaEvent>()
              .Requests.Declare<CommandToForward>()
              .Requests.Declare<MappedAlphaCommand>()
              .Requests.Declare<QueryToForward, string>()
              .Requests.Declare<MappedAlphaQuery, AlphaResponse>();
    }

    protected override void IncludeSubmodules(ISubstructureDefinition module, IServiceProvider services) 
    {
        module.AddSubmodule<BetaModule>(consumer =>
        {
            consumer.Events.ExposeWithInterception<EventToForward, EventInterceptor>()
                    .Events.ExposeWithInterception<MappedBetaEvent, MappedAlphaEvent, EventInterceptor>(
                        eventData => new(eventData.Id, eventData.PreInterceptions, eventData.PostInterceptions))
                    .Requests.ExposeWithInterception<CommandToForward, CommandInterceptor>()
                    .Requests.ExposeWithInterception<MappedBetaCommand, MappedAlphaCommand, CommandInterceptor>(
                        commandData => new(commandData.Id, commandData.PreInterceptions, commandData.PostInterceptions))
                    .Requests.ExposeWithInterception<QueryToForward, string, QueryInterceptor>()
                    .Requests.ExposeWithInterception<MappedBetaQuery, BetaResponse, MappedAlphaQuery, AlphaResponse, QueryInterceptor>(
                        queryData => new(queryData.Id, queryData.PreInterceptions, queryData.PostInterceptions), 
                        result => new(result.Content));
        });
    }

    protected override void RegisterServices(IServiceCollection services, IConfiguration configuration) { }
}
