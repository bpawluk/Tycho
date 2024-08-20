using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Tycho;
using Tycho.Contract.Inbox;
using Tycho.Contract.Outbox;
using Tycho.Messaging.Payload;
using Tycho.Structure;

namespace IntegrationTests.SendingMessagesDownTheHierarchy.SUT.Submodules;

// Incoming
internal record BetaDownstreamEvent(string Id) : IEvent;
internal record BetaDownstreamCommand(string Id) : IRequest;
internal record BetaDownstreamQuery(string Id) : IRequest<string>;

// Outgoing
internal record BetaUpstreamEvent(string Id) : IEvent;
internal record BetaUpstreamCommand(string Id) : IRequest;
internal record BetaUpstreamQuery(string Id) : IRequest<string>;

internal class BetaModule : TychoModule
{
    protected override void HandleIncomingMessages(IInboxDefinition module, IServiceProvider services)
    {
        var gammaModule = services.GetRequiredService<IModule<GammaModule>>();

        module.SubscribesTo<BetaDownstreamEvent>(eventData =>
        {
            gammaModule.Publish<GammaDownstreamEvent>(new(eventData.Id));
        });

        module.Executes<BetaDownstreamCommand>(commandData =>
        {
            gammaModule.Execute<GammaDownstreamCommand>(new(commandData.Id));
        });

        module.RespondsTo<BetaDownstreamQuery, string>(queryData =>
        {
            return gammaModule.Execute<GammaDownstreamQuery, string>(new(queryData.Id));
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
                thisModule.Publish<BetaUpstreamEvent>(new(eventData.Id));
            });

            consumer.HandleCommand<GammaUpstreamCommand>(commandData =>
            {
                thisModule.Execute<BetaUpstreamCommand>(new(commandData.Id));
            });

            consumer.HandleQuery<GammaUpstreamQuery, string>(queryData =>
            {
                return thisModule.Execute<BetaUpstreamQuery, string>(new(queryData.Id));
            });
        });
    }

    protected override void RegisterServices(IServiceCollection services, IConfiguration configuration) { }
}
