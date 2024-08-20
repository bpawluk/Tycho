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
internal record GammaDownstreamEvent(string Id) : IEvent;
internal record GammaDownstreamRequest(string Id) : IRequest;
internal record GammaDownstreamRequestWithResponse(string Id) : IRequest<string>;

// Outgoing
internal record GammaUpstreamEvent(string Id) : IEvent;
internal record GammaUpstreamRequest(string Id) : IRequest;
internal record GammaUpstreamRequestWithResponse(string Id) : IRequest<string>;

internal class GammaModule : TychoModule
{
    protected override void HandleIncomingMessages(IInboxDefinition module, IServiceProvider services)
    {
        var thisModule = services.GetRequiredService<IModule>();

        module.Events.Handle<GammaDownstreamEvent>(eventData =>
        {
            thisModule.Publish<GammaUpstreamEvent>(new(eventData.Id));
        });

        module.Requests.Handle<GammaDownstreamRequest>(requestData =>
        {
            thisModule.Execute<GammaUpstreamRequest>(new(requestData.Id));
        });

        module.Requests.Handle<GammaDownstreamRequestWithResponse, string>(requestData =>
        {
            return thisModule.Execute<GammaUpstreamRequestWithResponse, string>(new(requestData.Id));
        });
    }

    protected override void DeclareOutgoingMessages(IOutboxDefinition module, IServiceProvider services)
    {
        module.Events.Declare<GammaUpstreamEvent>()
              .Requests.Declare<GammaUpstreamRequest>()
              .Requests.Declare<GammaUpstreamRequestWithResponse, string>();
    }

    protected override void IncludeSubmodules(ISubstructureDefinition module, IServiceProvider services) { }

    protected override void RegisterServices(IServiceCollection services, IConfiguration configuration) { }
}
