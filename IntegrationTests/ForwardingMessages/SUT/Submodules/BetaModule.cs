using Microsoft.Extensions.DependencyInjection;
using System;
using Tycho;
using Tycho.Contract;
using Tycho.Messaging.Payload;
using Tycho.Structure;

namespace IntegrationTests.ForwardingMessages.SUT.Submodules;

// Incoming
internal record MappedBetaEvent(string Id) : IEvent;
internal record MappedBetaCommand(string Id) : ICommand;
internal record MappedBetaQuery(string Id) : IQuery<string>;
internal record MappedBetaQueryAndResponse(string Id) : IQuery<BetaResponse>;

// Outgoing
internal record BetaEvent(string Id) : IEvent;
internal record BetaEventToMap(string Id) : IEvent;
internal record BetaCommand(string Id) : ICommand;
internal record BetaCommandToMap(string Id) : ICommand;
internal record BetaQuery(string Id) : IQuery<string>;
internal record BetaQueryToMapMessage(string Id) : IQuery<string>;
internal record BetaQueryToMapMessageAndResponse(string Id) : IQuery<BetaResponse>;

internal record BetaResponse(string Content);

internal class BetaModule : TychoModule
{
    protected override void DeclareIncomingMessages(IInboxDefinition module, IServiceProvider services)
    {
        var thisModule = services.GetRequiredService<IModule>();
        module.SubscribesTo<AlphaEvent>(eventData => thisModule.Publish<BetaEvent>(new(eventData.Id)))
              .SubscribesTo<MappedBetaEvent>(eventData => thisModule.Publish<BetaEventToMap>(new(eventData.Id)))
              .Executes<AlphaCommand>(commandData => thisModule.Execute<BetaCommand>(new(commandData.Id)))
              .Executes<MappedBetaCommand>(commandData => thisModule.Execute<BetaCommandToMap>(new(commandData.Id)))
              .RespondsTo<AlphaQuery, string>(queryData =>
              {
                  return thisModule.Execute<BetaQuery, string>(new(queryData.Id));
              })
              .RespondsTo<MappedBetaQuery, string>(queryData =>
              {
                  return thisModule.Execute<BetaQueryToMapMessage, string>(new(queryData.Id));
              })
              .RespondsTo<MappedBetaQueryAndResponse, BetaResponse>(queryData =>
              {
                  return thisModule.Execute<BetaQueryToMapMessageAndResponse, BetaResponse>(new(queryData.Id));
              });
    }

    protected override void DeclareOutgoingMessages(IOutboxDefinition module, IServiceProvider services)
    {
        module.Publishes<BetaEvent>()
              .Publishes<BetaEventToMap>()
              .Sends<BetaCommand>()
              .Sends<BetaCommandToMap>()
              .Sends<BetaQuery, string>()
              .Sends<BetaQueryToMapMessage, string>()
              .Sends<BetaQueryToMapMessageAndResponse, BetaResponse>();
    }

    protected override void IncludeSubmodules(ISubstructureDefinition module, IServiceProvider services) { }

    protected override void RegisterServices(IServiceCollection services) { }
}
