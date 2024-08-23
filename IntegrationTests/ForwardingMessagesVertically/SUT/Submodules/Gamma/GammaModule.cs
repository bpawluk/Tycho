using IntegrationTests.ForwardingMessagesVertically.SUT.Submodules.Gamma.Handlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Tycho;
using Tycho.Contract.Inbox;
using Tycho.Contract.Outbox;
using Tycho.Structure;

namespace IntegrationTests.ForwardingMessagesVertically.SUT.Submodules.Gamma;

internal class GammaModule : TychoModule
{
    protected override void DeclareOutgoingMessages(IOutboxDefinition outbox, IServiceProvider services)
    {
        outbox.Events.Declare<EventToForward>()
              .Events.Declare<GammaOutEvent>();

        outbox.Requests.Declare<RequestToForward>()
              .Requests.Declare<GammaOutRequest>();

        outbox.Requests.Declare<RequestWithResponseToForward, string>()
              .Requests.Declare<GammaOutRequestWithResponse, GammaOutRequestWithResponse.Response>();
    }

    protected override void HandleIncomingMessages(IInboxDefinition inbox, IServiceProvider services)
    {
        inbox.Events.Handle<EventToForward, GammaEventHandler>()
             .Events.Handle<GammaInEvent, GammaEventHandler>();

        inbox.Requests.Handle<RequestToForward, GammaRequestHandler>()
             .Requests.Handle<GammaInRequest, GammaRequestHandler>();

        inbox.Requests.Handle<RequestWithResponseToForward, string, GammaRequestHandler>()
             .Requests.Handle<GammaInRequestWithResponse, GammaInRequestWithResponse.Response, GammaRequestHandler>();
    }

    protected override void IncludeSubmodules(ISubstructureDefinition structure, IServiceProvider services) { }

    protected override void RegisterServices(IServiceCollection services, IConfiguration configuration) { }
}
