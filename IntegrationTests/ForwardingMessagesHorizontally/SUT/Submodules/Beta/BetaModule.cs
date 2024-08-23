using IntegrationTests.ForwardingMessagesHorizontally.SUT.Submodules.Beta.Handlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Tycho;
using Tycho.Contract.Inbox;
using Tycho.Contract.Outbox;
using Tycho.Structure;

namespace IntegrationTests.ForwardingMessagesHorizontally.SUT.Submodules.Beta;

internal class BetaModule : TychoModule
{
    protected override void DeclareOutgoingMessages(IOutboxDefinition outbox, IServiceProvider services)
    {
        outbox.Events.Declare<EventToForward>()
              .Events.Declare<BetaOutEvent>();

        outbox.Requests.Declare<RequestToForward>()
              .Requests.Declare<BetaOutRequest>();

        outbox.Requests.Declare<RequestWithResponseToForward, string>()
              .Requests.Declare<BetaOutRequestWithResponse, BetaOutRequestWithResponse.Response>();
    }

    protected override void HandleIncomingMessages(IInboxDefinition inbox, IServiceProvider services)
    {
        inbox.Events.Handle<EventToForward, BetaEventHandler>()
             .Events.Handle<BetaInEvent, BetaEventHandler>();

        inbox.Requests.Handle<RequestToForward, BetaRequestHandler>()
             .Requests.Handle<BetaInRequest, BetaRequestHandler>();

        inbox.Requests.Handle<RequestWithResponseToForward, string, BetaRequestHandler>()
             .Requests.Handle<BetaInRequestWithResponse, BetaInRequestWithResponse.Response, BetaRequestHandler>();
    }

    protected override void IncludeSubmodules(ISubstructureDefinition structure, IServiceProvider services) { }

    protected override void RegisterServices(IServiceCollection services, IConfiguration configuration) { }
}
