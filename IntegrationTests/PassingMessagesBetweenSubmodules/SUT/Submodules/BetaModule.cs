using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Tycho;
using Tycho.Contract.Inbox;
using Tycho.Contract.Outbox;
using Tycho.Messaging.Payload;
using Tycho.Structure;

namespace IntegrationTests.PassingMessagesBetweenSubmodules.SUT.Submodules;

// Incoming
internal record FromAlphaEvent(string Id) : IEvent;
internal record FromAlphaRequest(string Id) : IRequest;
internal record FromAlphaRequestWithResponse(string Id) : IRequest<string>;

// Outgoing
internal record BetaEvent(string Id) : IEvent;
internal record BetaRequest(string Id) : IRequest;
internal record BetaRequestWithResponse(string Id) : IRequest<string>;

internal class BetaModule : TychoModule
{
    protected override void HandleIncomingMessages(IInboxDefinition module, IServiceProvider services)
    {
        var thisModule = services.GetRequiredService<IModule>();

        module.Events.Handle<FromAlphaEvent>(eventData =>
        {
            thisModule.Publish<BetaEvent>(new(eventData.Id));
        });

        module.Requests.Handle<FromAlphaRequest>(requestData =>
        {
            thisModule.Execute<BetaRequest>(new(requestData.Id));
        });

        module.Requests.Handle<FromAlphaRequestWithResponse, string>(requestData =>
        {
            return thisModule.Execute<BetaRequestWithResponse, string>(new(requestData.Id));
        });
    }

    protected override void DeclareOutgoingMessages(IOutboxDefinition module, IServiceProvider services)
    {
        module.Events.Declare<BetaEvent>()
              .Requests.Declare<BetaRequest>()
              .Requests.Declare<BetaRequestWithResponse, string>();
    }

    protected override void IncludeSubmodules(ISubstructureDefinition module, IServiceProvider services) { }

    protected override void RegisterServices(IServiceCollection services, IConfiguration configuration) { }
}
