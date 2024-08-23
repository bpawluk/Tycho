using IntegrationTests.SendingMessagesHorizontally.SUT.Submodules.Alpha.Handlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Tycho;
using Tycho.Contract.Inbox;
using Tycho.Contract.Outbox;
using Tycho.Structure;

namespace IntegrationTests.SendingMessagesHorizontally.SUT.Submodules.Alpha;

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
        inbox.Events.Handle<EventToSend, AlphaEventHandler>()
             .Events.Handle<AlphaInEvent, AlphaEventHandler>();

        inbox.Requests.Handle<RequestToSend, AlphaRequestHandler>()
             .Requests.Handle<AlphaInRequest, AlphaRequestHandler>();

        inbox.Requests.Handle<RequestWithResponseToSend, string, AlphaRequestHandler>()
             .Requests.Handle<AlphaInRequestWithResponse, AlphaInRequestWithResponse.Response, AlphaRequestHandler>();
    }

    protected override void IncludeSubmodules(ISubstructureDefinition structure, IServiceProvider services) { }

    protected override void RegisterServices(IServiceCollection services, IConfiguration configuration) { }
}
