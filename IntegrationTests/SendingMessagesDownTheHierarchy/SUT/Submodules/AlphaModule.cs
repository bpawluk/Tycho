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
internal record AlphaDownstreamRequest(string Id) : IRequest;
internal record AlphaDownstreamRequestWithResponse(string Id) : IRequest<string>;

// Outgoing
internal record AlphaUpstreamEvent(string Id) : IEvent;
internal record AlphaUpstreamRequest(string Id) : IRequest;
internal record AlphaUpstreamRequestWithResponse(string Id) : IRequest<string>;

internal class AlphaModule : TychoModule
{
    protected override void HandleIncomingMessages(IInboxDefinition module, IServiceProvider services)
    {
        var betaModule = services.GetRequiredService<IModule<BetaModule>>();

        module.Events.Handle<AlphaDownstreamEvent>(eventData =>
        {
            betaModule.Publish<BetaDownstreamEvent>(new(eventData.Id));
        });

        module.Requests.Handle<AlphaDownstreamRequest>(requestData =>
        {
            betaModule.Execute<BetaDownstreamRequest>(new(requestData.Id));
        });

        module.Requests.Handle<AlphaDownstreamRequestWithResponse, string>(requestData =>
        {
            return betaModule.Execute<BetaDownstreamRequestWithResponse, string>(new(requestData.Id));
        });
    }

    protected override void DeclareOutgoingMessages(IOutboxDefinition module, IServiceProvider services)
    {
        module.Events.Declare<AlphaUpstreamEvent>()
              .Requests.Declare<AlphaUpstreamRequest>()
              .Requests.Declare<AlphaUpstreamRequestWithResponse, string>();
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

            consumer.Requests.Handle<BetaUpstreamRequest>(requestData =>
            {
                thisModule.Execute<AlphaUpstreamRequest>(new(requestData.Id));
            });

            consumer.Requests.Handle<BetaUpstreamRequestWithResponse, string>(requestData =>
            {
                return thisModule.Execute<AlphaUpstreamRequestWithResponse, string>(new(requestData.Id));
            });
        });
    }

    protected override void RegisterServices(IServiceCollection services, IConfiguration configuration) { }
}
