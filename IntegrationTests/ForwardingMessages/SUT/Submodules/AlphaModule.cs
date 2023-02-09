using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Tycho;
using Tycho.Contract;
using Tycho.Messaging.Payload;
using Tycho.Structure;

namespace IntegrationTests.ForwardingMessages.SUT.Submodules;

// Incoming
internal record MappedAlphaEvent(string Id) : IEvent;
internal record MappedAlphaCommand(string Id) : ICommand;
internal record MappedAlphaQuery(string Id) : IQuery<string>;
internal record MappedAlphaQueryAndResponse(string Id) : IQuery<AlphaResponse>;

// Outgoing
internal record AlphaEvent(string Id) : IEvent;
internal record AlphaEventToMap(string Id) : IEvent;
internal record AlphaCommand(string Id) : ICommand;
internal record AlphaCommandToMap(string Id) : ICommand;
internal record AlphaQuery(string Id) : IQuery<string>;
internal record AlphaQueryToMapMessage(string Id) : IQuery<string>;
internal record AlphaQueryToMapMessageAndResponse(string Id) : IQuery<AlphaResponse>;

internal record AlphaResponse(string Content);

internal class AlphaModule : TychoModule
{
    protected override void DeclareIncomingMessages(IInboxDefinition module, IServiceProvider services)
    {
        var thisModule = services.GetRequiredService<IModule>();
        module.SubscribesTo<EventToPass>(eventData => thisModule.Publish<AlphaEvent>(new(eventData.Id)))
              .SubscribesTo<MappedAlphaEvent>(eventData => thisModule.Publish<AlphaEventToMap>(new(eventData.Id)))
              .Executes<CommandToForward>(commandData => thisModule.Execute<AlphaCommand>(new(commandData.Id)))
              .Executes<MappedAlphaCommand>(commandData => thisModule.Execute<AlphaCommandToMap>(new(commandData.Id)))
              .RespondsTo<QueryToForward, string>(queryData =>
              {
                  return thisModule.Execute<AlphaQuery, string>(new(queryData.Id));
              })
              .RespondsTo<MappedAlphaQuery, string>(queryData =>
              {
                  return thisModule.Execute<AlphaQueryToMapMessage, string>(new(queryData.Id));
              })
              .RespondsTo<MappedAlphaQueryAndResponse, AlphaResponse>(queryData =>
              {
                  return thisModule.Execute<AlphaQueryToMapMessageAndResponse, AlphaResponse>(new(queryData.Id));
              });
    }

    protected override void DeclareOutgoingMessages(IOutboxDefinition module, IServiceProvider services)
    {
        module.Publishes<AlphaEvent>()
              .Publishes<AlphaEventToMap>()
              .Sends<AlphaCommand>()
              .Sends<AlphaCommandToMap>()
              .Sends<AlphaQuery, string>()
              .Sends<AlphaQueryToMapMessage, string>()
              .Sends<AlphaQueryToMapMessageAndResponse, AlphaResponse>();
    }

    protected override void IncludeSubmodules(ISubstructureDefinition module, IServiceProvider services) { }

    protected override void RegisterServices(IServiceCollection services, IConfiguration configuration) { }
}
