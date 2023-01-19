using Microsoft.Extensions.DependencyInjection;
using System;
using Tycho;
using Tycho.Contract;
using Tycho.Messaging.Payload;
using Tycho.Structure;

namespace Test.Integration.SendingMessagesDownTheHierarchy.SUT.Submodules;

// Incoming
internal record AlphaDownstreamEvent(string Id) : IEvent;
internal record AlphaDownstreamCommand(string Id) : ICommand;
internal record AlphaDownstreamQuery(string Id) : IQuery<string>;

// Outgoing
internal record AlphaUpstreamEvent(string Id) : IEvent;
internal record AlphaUpstreamCommand(string Id) : ICommand;
internal record AlphaUpstreamQuery(string Id) : IQuery<string>;

internal class AlphaModule : TychoModule
{
    protected override void DeclareIncomingMessages(IInboxDefinition module, IServiceProvider services)
    {
        var betaModule = services.GetRequiredService<ISubmodule<BetaModule>>();

        module.SubscribesTo<AlphaDownstreamEvent>(eventData =>
        {
            betaModule.PublishEvent<BetaDownstreamEvent>(new(eventData.Id));
        });

        module.Executes<AlphaDownstreamCommand>(commandData =>
        {
            betaModule.ExecuteCommand<BetaDownstreamCommand>(new(commandData.Id));
        });

        module.RespondsTo<AlphaDownstreamQuery, string>(queryData =>
        {
            return betaModule.ExecuteQuery<BetaDownstreamQuery, string>(new(queryData.Id));
        });
    }

    protected override void DeclareOutgoingMessages(IOutboxDefinition module, IServiceProvider services)
    {
        module.Publishes<AlphaUpstreamEvent>()
              .Sends<AlphaUpstreamCommand>()
              .Sends<AlphaUpstreamQuery, string>();
    }

    protected override void IncludeSubmodules(ISubstructureDefinition module, IServiceProvider services)
    {
        module.AddSubmodule<BetaModule>(consumer =>
        {
            var thisModule = services.GetRequiredService<IModule>();

            consumer.HandleEvent<BetaUpstreamEvent>(eventData =>
            {
                thisModule.PublishEvent<AlphaUpstreamEvent>(new(eventData.Id));
            });

            consumer.HandleCommand<BetaUpstreamCommand>(commandData =>
            {
                thisModule.ExecuteCommand<AlphaUpstreamCommand>(new(commandData.Id));
            });

            consumer.HandleQuery<BetaUpstreamQuery, string>(queryData =>
            {
                return thisModule.ExecuteQuery<AlphaUpstreamQuery, string>(new(queryData.Id));
            });
        });
    }

    protected override void RegisterServices(IServiceCollection services) { }
}
