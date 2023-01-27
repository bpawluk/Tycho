using Microsoft.Extensions.DependencyInjection;
using System;
using Test.Integration.ForwardingMessages.SUT.Submodules;
using Tycho;
using Tycho.Contract;
using Tycho.Messaging.Payload;
using Tycho.Structure;

namespace Test.Integration.ForwardingMessages.SUT;

// Incoming
internal record EventToPass(string Id) : IEvent;
internal record EventToPassWithMapping(string Id) : IEvent;
internal record CommandToForward(string Id) : ICommand;
internal record CommandToForwardWithMapping(string Id) : ICommand;
internal record QueryToForward(string Id) : IQuery<string>;
internal record QueryToForwardWithMessageMapping(string Id) : IQuery<string>;
internal record QueryToForwardWithMessageAndResponseMapping(string Id) : IQuery<string>;

// Outgoing
internal record MappedEvent(string Id) : IEvent;
internal record MappedCommand(string Id) : ICommand;
internal record MappedQuery(string Id) : IQuery<string>;
internal record MappedQueryAndResponse(string Id) : IQuery<string>;

internal class AppModule : TychoModule
{
    protected override void DeclareIncomingMessages(IInboxDefinition module, IServiceProvider services)
    {
        module.PassesOn<EventToPass, AlphaModule>()
              .PassesOn<EventToPassWithMapping, MappedAlphaEvent, AlphaModule>(eventData => new(eventData.Id))
              .Forwards<CommandToForward, AlphaModule>()
              .Forwards<CommandToForwardWithMapping, MappedAlphaCommand, AlphaModule>(commandData => new(commandData.Id))
              .Forwards<QueryToForward, string, AlphaModule>()
              .Forwards<QueryToForwardWithMessageMapping, MappedAlphaQuery, string, AlphaModule>(queryData =>
              {
                  return new(queryData.Id);
              })
              .Forwards<QueryToForwardWithMessageAndResponseMapping, string, 
                  MappedAlphaQueryAndResponse, AlphaResponse, 
                  AlphaModule>(queryData => new(queryData.Id), response => response.Content);
    }

    protected override void DeclareOutgoingMessages(IOutboxDefinition module, IServiceProvider services)
    {
        module.Publishes<BetaEvent>()
              .Publishes<MappedEvent>()
              .Sends<BetaCommand>()
              .Sends<MappedCommand>()
              .Sends<BetaQuery, string>()
              .Sends<MappedQuery, string>()
              .Sends<MappedQueryAndResponse, string>();
    }

    protected override void IncludeSubmodules(ISubstructureDefinition module, IServiceProvider services)
    {
        module.AddSubmodule<AlphaModule>(consumer =>
        {
            consumer.PassOn<AlphaEvent, BetaModule>()
                    .PassOn<AlphaEventToMap, MappedBetaEvent, BetaModule>(eventData => new(eventData.Id))
                    .Forward<AlphaCommand, BetaModule>()
                    .Forward<AlphaCommandToMap, MappedBetaCommand, BetaModule>(commandData => new(commandData.Id))
                    .Forward<AlphaQuery, string, BetaModule>()
                    .Forward<AlphaQueryToMapMessage, MappedBetaQuery, string, BetaModule>(queryData => new(queryData.Id))
                    .Forward<AlphaQueryToMapMessageAndResponse, AlphaResponse, 
                        MappedBetaQueryAndResponse, BetaResponse, 
                        BetaModule>(queryData => new(queryData.Id), response => new(response.Content));
        });

        module.AddSubmodule<BetaModule>(consumer =>
        {
            consumer.ExposeEvent<BetaEvent>()
                    .ExposeEvent<BetaEventToMap, MappedEvent>(eventData => new(eventData.Id))
                    .ExposeCommand<BetaCommand>()
                    .ExposeCommand<BetaCommandToMap, MappedCommand>(commandData => new(commandData.Id))
                    .ExposeQuery<BetaQuery, string>()
                    .ExposeQuery<BetaQueryToMapMessage, MappedQuery, string>(queryData => new(queryData.Id))
                    .ExposeQuery<BetaQueryToMapMessageAndResponse, BetaResponse, MappedQueryAndResponse, string>(
                        queryData => new(queryData.Id), response => new(response));
        });
    }

    protected override void RegisterServices(IServiceCollection services) { }
}
