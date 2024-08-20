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
internal record FromBetaEvent(string Id) : IEvent;
internal record FromBetaRequest(string Id) : IRequest;
internal record FromBetaRequestWithResponse(string Id) : IRequest<string>;

// Outgoing
internal record GammaEvent(string Id) : IEvent;
internal record GammaRequest(string Id) : IRequest;
internal record GammaRequestWithResponse(string Id) : IRequest<string>;

internal class GammaModule : TychoModule
{
    protected override void HandleIncomingMessages(IInboxDefinition module, IServiceProvider services)
    {
        var thisModule = services.GetRequiredService<IModule>();

        module.Events.Handle<FromBetaEvent>(eventData =>
        {
            thisModule.Publish<GammaEvent>(new(eventData.Id));
        });

        module.Requests.Handle<FromBetaRequest>(requestData =>
        {
            thisModule.Execute<GammaRequest>(new(requestData.Id));
        });

        module.Requests.Handle<FromBetaRequestWithResponse, string>(requestData =>
        {
            return thisModule.Execute<GammaRequestWithResponse, string>(new(requestData.Id));
        });
    }

    protected override void DeclareOutgoingMessages(IOutboxDefinition module, IServiceProvider services)
    {
        module.Events.Declare<GammaEvent>()
              .Requests.Declare<GammaRequest>()
              .Requests.Declare<GammaRequestWithResponse, string>();
    }

    protected override void IncludeSubmodules(ISubstructureDefinition module, IServiceProvider services) { }

    protected override void RegisterServices(IServiceCollection services, IConfiguration configuration) { }
}
