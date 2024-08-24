using IntegrationTests.SendingMessagesVertically.SUT.Submodules.Beta.Handlers;
using IntegrationTests.SendingMessagesVertically.SUT.Submodules.Gamma;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Tycho;
using Tycho.Contract.Inbox;
using Tycho.Contract.Outbox;
using Tycho.Structure;

namespace IntegrationTests.SendingMessagesVertically.SUT.Submodules.Beta;

internal class BetaModule : TychoModule
{
    protected override void DeclareOutgoingMessages(IOutboxDefinition outbox, IServiceProvider services)
    {
        outbox.Events.Declare<EventToSend>()
              .Events.Declare<BetaOutEvent>();

        outbox.Requests.Declare<RequestToSend>()
              .Requests.Declare<BetaOutRequest>();

        outbox.Requests.Declare<RequestWithResponseToSend, string>()
              .Requests.Declare<BetaOutRequestWithResponse, BetaOutRequestWithResponse.Response>();
    }

    protected override void HandleIncomingMessages(IInboxDefinition inbox, IServiceProvider services)
    {
        inbox.Events.Handle<EventToSend, BetaHandler>()
             .Events.Handle<BetaInEvent, BetaHandler>();

        inbox.Requests.Handle<RequestToSend, BetaHandler>()
             .Requests.Handle<BetaInRequest, BetaHandler>();

        inbox.Requests.Handle<RequestWithResponseToSend, string, BetaHandler>()
             .Requests.Handle<BetaInRequestWithResponse, BetaInRequestWithResponse.Response, BetaHandler>();
    }

    protected override void IncludeSubmodules(ISubstructureDefinition structure, IServiceProvider services) 
    {
        structure.AddSubmodule<GammaModule>(betaOutbox =>
        {
            betaOutbox.Events.Handle<EventToSend, GammaHandler>()
                      .Events.Handle<GammaOutEvent, GammaHandler>();

            betaOutbox.Requests.Handle<RequestToSend, GammaHandler>()
                      .Requests.Handle<GammaOutRequest, GammaHandler>();

            betaOutbox.Requests.Handle<RequestWithResponseToSend, string, GammaHandler>()
                      .Requests.Handle<GammaOutRequestWithResponse, GammaOutRequestWithResponse.Response, GammaHandler>();
        });
    }

    protected override void RegisterServices(IServiceCollection services, IConfiguration configuration) { }
}
