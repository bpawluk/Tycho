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

internal record CommandToForward(string Id, int preInterceptions, int postInterceptions) : ICommand
{
    public int PreInterceptions { get; set; } = preInterceptions;
    public int PostInterceptions { get; set; } = postInterceptions;
};

internal record CommandToForwardWithMapping(string Id, int preInterceptions, int postInterceptions) : ICommand
{
    public int PreInterceptions { get; set; } = preInterceptions;
    public int PostInterceptions { get; set; } = postInterceptions;
};

internal record QueryToForward(string Id, int preInterceptions, int postInterceptions) : IQuery<string>
{
    public int PreInterceptions { get; set; } = preInterceptions;
    public int PostInterceptions { get; set; } = postInterceptions;
};

internal record QueryToForwardWithMapping(string Id, int preInterceptions, int postInterceptions) : IQuery<string>
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

internal record MappedCommand(string Id, int preInterceptions, int postInterceptions) : ICommand
{
    public int PreInterceptions { get; set; } = preInterceptions;
    public int PostInterceptions { get; set; } = postInterceptions;
};

internal record MappedQuery(string Id, int preInterceptions, int postInterceptions) : IQuery<string>
{
    public int PreInterceptions { get; set; } = preInterceptions;
    public int PostInterceptions { get; set; } = postInterceptions;
};

internal class AppModule : TychoModule
{
    protected override void DeclareIncomingMessages(IInboxDefinition module, IServiceProvider services)
    {
        module.ForwardsEvent<EventToForward, EventInterceptor, AlphaModule>()
              .ForwardsEvent<EventToForwardWithMapping, MappedAlphaEvent, EventInterceptor, AlphaModule>(
                  eventData => new(eventData.Id, eventData.PreInterceptions, eventData.PostInterceptions))
              .ForwardsCommand<CommandToForward, CommandInterceptor, AlphaModule>()
              .ForwardsCommand<CommandToForwardWithMapping, MappedAlphaCommand, CommandInterceptor, AlphaModule>(
                  commandData => new(commandData.Id, commandData.PreInterceptions, commandData.PostInterceptions))
              .ForwardsQuery<QueryToForward, string, QueryInterceptor, AlphaModule>()
              .ForwardsQuery<QueryToForwardWithMapping, string, MappedAlphaQuery, AlphaResponse, QueryInterceptor, AlphaModule>(
                  queryData => new(queryData.Id, queryData.PreInterceptions, queryData.PostInterceptions),
                  response => response.Content);
    }

    protected override void DeclareOutgoingMessages(IOutboxDefinition module, IServiceProvider services)
    {
        module.Publishes<EventToForward>()
              .Publishes<MappedEvent>()
              .Sends<CommandToForward>()
              .Sends<MappedCommand>()
              .Sends<QueryToForward, string>()
              .Sends<MappedQuery, string>();
    }

    protected override void IncludeSubmodules(ISubstructureDefinition module, IServiceProvider services)
    {
        module.AddSubmodule<AlphaModule>(consumer =>
        {
            consumer.ExposeEvent<EventToForward, EventInterceptor>()
                    .ExposeEvent<MappedAlphaEvent, MappedEvent, EventInterceptor>(
                        eventData => new(eventData.Id, eventData.PreInterceptions, eventData.PostInterceptions))
                    .ExposeCommand<CommandToForward, CommandInterceptor>()
                    .ExposeCommand<MappedAlphaCommand, MappedCommand, CommandInterceptor>(
                        commandData => new(commandData.Id, commandData.PreInterceptions, commandData.PostInterceptions))
                    .ExposeQuery<QueryToForward, string, QueryInterceptor>()
                    .ExposeQuery<MappedAlphaQuery, AlphaResponse, MappedQuery, string, QueryInterceptor>(
                        queryData => new(queryData.Id, queryData.PreInterceptions, queryData.PostInterceptions),
                        result => new(result));
        });
    }

    protected override void RegisterServices(IServiceCollection services, IConfiguration configuration) { }
}
