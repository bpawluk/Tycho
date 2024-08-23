using IntegrationTests.ForwardingMessagesHorizontally.SUT.Submodules.Alpha.Handlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Tycho;
using Tycho.Contract.Inbox;
using Tycho.Contract.Outbox;
using Tycho.Structure;

namespace IntegrationTests.ForwardingMessagesHorizontally.SUT.Submodules.Alpha;

internal class AlphaModule : TychoModule
{
    protected override void DeclareOutgoingMessages(IOutboxDefinition outbox, IServiceProvider services)
    {
        outbox.Events.Declare<EventToForward>()
              .Events.Declare<AlphaOutEvent>();

        outbox.Requests.Declare<RequestToForward>()
              .Requests.Declare<AlphaOutRequest>();

        outbox.Requests.Declare<RequestWithResponseToForward, string>()
              .Requests.Declare<AlphaOutRequestWithResponse, AlphaOutRequestWithResponse.Response>();
    }

    protected override void HandleIncomingMessages(IInboxDefinition inbox, IServiceProvider services)
    {
        inbox.Events.Handle<EventToForward, AlphaEventHandler>()
             .Events.Handle<AlphaInEvent, AlphaEventHandler>();

        inbox.Requests.Handle<RequestToForward, AlphaRequestHandler>()
             .Requests.Handle<AlphaInRequest, AlphaRequestHandler>();

        inbox.Requests.Handle<RequestWithResponseToForward, string, AlphaRequestHandler>()
             .Requests.Handle<AlphaInRequestWithResponse, AlphaInRequestWithResponse.Response, AlphaRequestHandler>();
    }

    protected override void IncludeSubmodules(ISubstructureDefinition structure, IServiceProvider services) { }

    protected override void RegisterServices(IServiceCollection services, IConfiguration configuration) { }
}
