using IntegrationTests.SendingMessagesVertically.SUT.Submodules.Alpha.Handlers;
using IntegrationTests.SendingMessagesVertically.SUT.Submodules.Beta;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Tycho;
using Tycho.Contract.Inbox;
using Tycho.Contract.Outbox;
using Tycho.Structure;

namespace IntegrationTests.SendingMessagesVertically.SUT.Submodules.Alpha;

internal class AlphaModule : TychoModule
{
    protected override void DeclareOutgoingMessages(IOutboxDefinition outbox, IServiceProvider services)
    {
        outbox.Events.Declare<EventToSend>()
              .Events.Declare<AlphaOutEvent>();

        outbox.Requests.Declare<RequestToSend>()
              .Requests.Declare<AlphaOutRequest>();

        outbox.Requests.Declare<RequestWithResponseToSend, string>()
              .Requests.Declare<AlphaOutRequestWithResponse, AlphaOutRequestWithResponse.Response>();
    }

    protected override void HandleIncomingMessages(IInboxDefinition inbox, IServiceProvider services)
    {
        inbox.Events.Handle<EventToSend, AlphaHandler>()
             .Events.Handle<AlphaInEvent, AlphaHandler>();

        inbox.Requests.Handle<RequestToSend, AlphaHandler>()
             .Requests.Handle<AlphaInRequest, AlphaHandler>();

        inbox.Requests.Handle<RequestWithResponseToSend, string, AlphaHandler>()
             .Requests.Handle<AlphaInRequestWithResponse, AlphaInRequestWithResponse.Response, AlphaHandler>();
    }

    protected override void IncludeSubmodules(ISubstructureDefinition structure, IServiceProvider services) 
    {
        structure.AddSubmodule<BetaModule>(betaOutbox =>
        {
            betaOutbox.Events.Handle<EventToSend, BetaHandler>()
                      .Events.Handle<BetaOutEvent, BetaHandler>();

            betaOutbox.Requests.Handle<RequestToSend, BetaHandler>()
                      .Requests.Handle<BetaOutRequest, BetaHandler>();

            betaOutbox.Requests.Handle<RequestWithResponseToSend, string, BetaHandler>()
                      .Requests.Handle<BetaOutRequestWithResponse, BetaOutRequestWithResponse.Response, BetaHandler>();
        });
    }

    protected override void RegisterServices(IServiceCollection services, IConfiguration configuration) { }
}
