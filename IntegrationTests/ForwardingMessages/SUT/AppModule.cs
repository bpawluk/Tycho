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

internal record CommandToForward(string Id, int preInterceptions, int postInterceptions) : IRequest
{
    public int PreInterceptions { get; set; } = preInterceptions;
    public int PostInterceptions { get; set; } = postInterceptions;
};

internal record CommandToForwardWithMapping(string Id, int preInterceptions, int postInterceptions) : IRequest
{
    public int PreInterceptions { get; set; } = preInterceptions;
    public int PostInterceptions { get; set; } = postInterceptions;
};

internal record QueryToForward(string Id, int preInterceptions, int postInterceptions) : IRequest<string>
{
    public int PreInterceptions { get; set; } = preInterceptions;
    public int PostInterceptions { get; set; } = postInterceptions;
};

internal record QueryToForwardWithMapping(string Id, int preInterceptions, int postInterceptions) : IRequest<string>
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

internal record MappedCommand(string Id, int preInterceptions, int postInterceptions) : IRequest
{
    public int PreInterceptions { get; set; } = preInterceptions;
    public int PostInterceptions { get; set; } = postInterceptions;
};

internal record MappedQuery(string Id, int preInterceptions, int postInterceptions) : IRequest<string>
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
              .Requests.ForwardWithInterception<CommandToForward, CommandInterceptor, AlphaModule>()
              .Requests.ForwardWithInterception<CommandToForwardWithMapping, MappedAlphaCommand, CommandInterceptor, AlphaModule>(
                  commandData => new(commandData.Id, commandData.PreInterceptions, commandData.PostInterceptions))
              .Requests.ForwardWithInterception<QueryToForward, string, QueryInterceptor, AlphaModule>()
              .Requests.ForwardWithInterception<QueryToForwardWithMapping, string, MappedAlphaQuery, AlphaResponse, QueryInterceptor, AlphaModule>(
                  queryData => new(queryData.Id, queryData.PreInterceptions, queryData.PostInterceptions),
                  response => response.Content);
    }

    protected override void DeclareOutgoingMessages(IOutboxDefinition module, IServiceProvider services)
    {
        module.Events.Declare<EventToForward>()
              .Events.Declare<MappedEvent>()
              .Requests.Declare<CommandToForward>()
              .Requests.Declare<MappedCommand>()
              .Requests.Declare<QueryToForward, string>()
              .Requests.Declare<MappedQuery, string>();
    }

    protected override void IncludeSubmodules(ISubstructureDefinition module, IServiceProvider services)
    {
        module.AddSubmodule<AlphaModule>(consumer =>
        {
            consumer.Events.ExposeWithInterception<EventToForward, EventInterceptor>()
                    .Events.ExposeWithInterception<MappedAlphaEvent, MappedEvent, EventInterceptor>(
                        eventData => new(eventData.Id, eventData.PreInterceptions, eventData.PostInterceptions))
                    .Requests.ExposeWithInterception<CommandToForward, CommandInterceptor>()
                    .Requests.ExposeWithInterception<MappedAlphaCommand, MappedCommand, CommandInterceptor>(
                        commandData => new(commandData.Id, commandData.PreInterceptions, commandData.PostInterceptions))
                    .Requests.ExposeWithInterception<QueryToForward, string, QueryInterceptor>()
                    .Requests.ExposeWithInterception<MappedAlphaQuery, AlphaResponse, MappedQuery, string, QueryInterceptor>(
                        queryData => new(queryData.Id, queryData.PreInterceptions, queryData.PostInterceptions),
                        result => new(result));
        });
    }

    protected override void RegisterServices(IServiceCollection services, IConfiguration configuration) { }
}
