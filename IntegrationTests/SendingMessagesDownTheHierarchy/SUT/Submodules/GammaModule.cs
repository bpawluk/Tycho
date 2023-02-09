using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Tycho;
using Tycho.Contract;
using Tycho.Messaging.Payload;
using Tycho.Structure;

namespace IntegrationTests.SendingMessagesDownTheHierarchy.SUT.Submodules;

// Incoming
internal record GammaDownstreamEvent(string Id) : IEvent;
internal record GammaDownstreamCommand(string Id) : ICommand;
internal record GammaDownstreamQuery(string Id) : IQuery<string>;

// Outgoing
internal record GammaUpstreamEvent(string Id) : IEvent;
internal record GammaUpstreamCommand(string Id) : ICommand;
internal record GammaUpstreamQuery(string Id) : IQuery<string>;

internal class GammaModule : TychoModule
{
    protected override void DeclareIncomingMessages(IInboxDefinition module, IServiceProvider services)
    {
        var thisModule = services.GetRequiredService<IModule>();

        module.SubscribesTo<GammaDownstreamEvent>(eventData =>
        {
            thisModule.Publish<GammaUpstreamEvent>(new(eventData.Id));
        });

        module.Executes<GammaDownstreamCommand>(commandData =>
        {
            thisModule.Execute<GammaUpstreamCommand>(new(commandData.Id));
        });

        module.RespondsTo<GammaDownstreamQuery, string>(queryData =>
        {
            return thisModule.Execute<GammaUpstreamQuery, string>(new(queryData.Id));
        });
    }

    protected override void DeclareOutgoingMessages(IOutboxDefinition module, IServiceProvider services)
    {
        module.Publishes<GammaUpstreamEvent>()
              .Sends<GammaUpstreamCommand>()
              .Sends<GammaUpstreamQuery, string>();
    }

    protected override void IncludeSubmodules(ISubstructureDefinition module, IServiceProvider services) { }

    protected override void RegisterServices(IServiceCollection services, IConfiguration configuration) { }
}
