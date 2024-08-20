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
internal record BetaDownstreamRequest(string Id) : IRequest;
internal record BetaDownstreamRequestWithResponse(string Id) : IRequest<string>;

// Outgoing
internal record BetaUpstreamEvent(string Id) : IEvent;
internal record BetaUpstreamRequest(string Id) : IRequest;
internal record BetaUpstreamRequestWithResponse(string Id) : IRequest<string>;

internal class BetaModule : TychoModule
{
    protected override void HandleIncomingMessages(IInboxDefinition module, IServiceProvider services)
    {
        var gammaModule = services.GetRequiredService<IModule<GammaModule>>();

        module.Events.Handle<BetaDownstreamEvent>(eventData =>
        {
            gammaModule.Publish<GammaDownstreamEvent>(new(eventData.Id));
        });

        module.Requests.Handle<BetaDownstreamRequest>(requestData =>
        {
            gammaModule.Execute<GammaDownstreamRequest>(new(requestData.Id));
        });

        module.Requests.Handle<BetaDownstreamRequestWithResponse, string>(requestData =>
        {
            return gammaModule.Execute<GammaDownstreamRequestWithResponse, string>(new(requestData.Id));
        });
    }

    protected override void DeclareOutgoingMessages(IOutboxDefinition module, IServiceProvider services)
    {
        module.Events.Declare<BetaUpstreamEvent>()
              .Requests.Declare<BetaUpstreamRequest>()
              .Requests.Declare<BetaUpstreamRequestWithResponse, string>();
    }

    protected override void IncludeSubmodules(ISubstructureDefinition module, IServiceProvider services)
    {
        module.AddSubmodule<GammaModule>(consumer =>
        {
            var thisModule = services.GetRequiredService<IModule>();

            consumer.Events.Handle<GammaUpstreamEvent>(eventData =>
            {
                thisModule.Publish<BetaUpstreamEvent>(new(eventData.Id));
            });

            consumer.Requests.Handle<GammaUpstreamRequest>(requestData =>
            {
                thisModule.Execute<BetaUpstreamRequest>(new(requestData.Id));
            });

            consumer.Requests.Handle<GammaUpstreamRequestWithResponse, string>(requestData =>
            {
                return thisModule.Execute<BetaUpstreamRequestWithResponse, string>(new(requestData.Id));
            });
        });
    }

    protected override void RegisterServices(IServiceCollection services, IConfiguration configuration) { }
}
