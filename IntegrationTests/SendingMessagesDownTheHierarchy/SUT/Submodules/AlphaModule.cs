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
internal record AlphaDownstreamEvent(string Id) : IEvent;
internal record AlphaDownstreamCommand(string Id) : IRequest;
internal record AlphaDownstreamQuery(string Id) : IRequest<string>;

// Outgoing
internal record AlphaUpstreamEvent(string Id) : IEvent;
internal record AlphaUpstreamCommand(string Id) : IRequest;
internal record AlphaUpstreamQuery(string Id) : IRequest<string>;

internal class AlphaModule : TychoModule
{
    protected override void HandleIncomingMessages(IInboxDefinition module, IServiceProvider services)
    {
        var betaModule = services.GetRequiredService<IModule<BetaModule>>();

        module.Events.Handle<AlphaDownstreamEvent>(eventData =>
        {
            betaModule.Publish<BetaDownstreamEvent>(new(eventData.Id));
        });

        module.Requests.Handle<AlphaDownstreamCommand>(commandData =>
        {
            betaModule.Execute<BetaDownstreamCommand>(new(commandData.Id));
        });

        module.Requests.Handle<AlphaDownstreamQuery, string>(queryData =>
        {
            return betaModule.Execute<BetaDownstreamQuery, string>(new(queryData.Id));
        });
    }

    protected override void DeclareOutgoingMessages(IOutboxDefinition module, IServiceProvider services)
    {
        module.Events.Declare<AlphaUpstreamEvent>()
              .Requests.Declare<AlphaUpstreamCommand>()
              .Requests.Declare<AlphaUpstreamQuery, string>();
    }

    protected override void IncludeSubmodules(ISubstructureDefinition module, IServiceProvider services)
    {
        module.AddSubmodule<BetaModule>(consumer =>
        {
            var thisModule = services.GetRequiredService<IModule>();

            consumer.Events.Handle<BetaUpstreamEvent>(eventData =>
            {
                thisModule.Publish<AlphaUpstreamEvent>(new(eventData.Id));
            });

            consumer.Requests.Handle<BetaUpstreamCommand>(commandData =>
            {
                thisModule.Execute<AlphaUpstreamCommand>(new(commandData.Id));
            });

            consumer.Requests.Handle<BetaUpstreamQuery, string>(queryData =>
            {
                return thisModule.Execute<AlphaUpstreamQuery, string>(new(queryData.Id));
            });
        });
    }

    protected override void RegisterServices(IServiceCollection services, IConfiguration configuration) { }
}
