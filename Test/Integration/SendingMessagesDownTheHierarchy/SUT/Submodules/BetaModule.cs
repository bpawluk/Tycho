using Microsoft.Extensions.DependencyInjection;
using System;
using Tycho;
using Tycho.Contract;
using Tycho.Messaging.Payload;
using Tycho.Structure;

namespace Test.Integration.SendingMessagesDownTheHierarchy.SUT.Submodules;

// Incoming
internal record BetaDownstreamEvent(string Id) : IEvent;
internal record BetaDownstreamCommand(string Id) : ICommand;
internal record BetaDownstreamQuery(string Id) : IQuery<string>;

// Outgoing
internal record BetaUpstreamEvent(string Id) : IEvent;
internal record BetaUpstreamCommand(string Id) : ICommand;
internal record BetaUpstreamQuery(string Id) : IQuery<string>;

internal class BetaModule : TychoModule
{
    protected override void DeclareIncomingMessages(IInboxDefinition module, IServiceProvider services)
    {
        var gammaModule = services.GetRequiredService<ISubmodule<GammaModule>>();

        module.SubscribesTo<BetaDownstreamEvent>(eventData =>
        {
            gammaModule.PublishEvent<GammaDownstreamEvent>(new(eventData.Id));
        });

        module.Executes<BetaDownstreamCommand>(commandData =>
        {
            gammaModule.ExecuteCommand<GammaDownstreamCommand>(new(commandData.Id));
        });

        module.RespondsTo<BetaDownstreamQuery, string>(queryData =>
        {
            return gammaModule.ExecuteQuery<GammaDownstreamQuery, string>(new(queryData.Id));
        });
    }

    protected override void DeclareOutgoingMessages(IOutboxDefinition module, IServiceProvider services)
    {
        module.Publishes<BetaUpstreamEvent>()
              .Sends<BetaUpstreamCommand>()
              .Sends<BetaUpstreamQuery, string>();
    }

    protected override void IncludeSubmodules(ISubstructureDefinition module, IServiceProvider services)
    {
        module.AddSubmodule<GammaModule>(consumer =>
        {
            var thisModule = services.GetRequiredService<IModule>();

            consumer.HandleEvent<GammaUpstreamEvent>(eventData =>
            {
                thisModule.PublishEvent<BetaUpstreamEvent>(new(eventData.Id));
            });

            consumer.HandleCommand<GammaUpstreamCommand>(commandData =>
            {
                thisModule.ExecuteCommand<BetaUpstreamCommand>(new(commandData.Id));
            });

            consumer.HandleQuery<GammaUpstreamQuery, string>(queryData =>
            {
                return thisModule.ExecuteQuery<BetaUpstreamQuery, string>(new(queryData.Id));
            });
        });
    }

    protected override void RegisterServices(IServiceCollection services) { }
}
