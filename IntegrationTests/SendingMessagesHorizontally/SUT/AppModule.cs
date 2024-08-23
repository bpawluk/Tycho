using IntegrationTests.SendingMessagesHorizontally.SUT.Handlers;
using IntegrationTests.SendingMessagesHorizontally.SUT.Submodules.Alpha;
using IntegrationTests.SendingMessagesHorizontally.SUT.Submodules.Beta;
using IntegrationTests.SendingMessagesHorizontally.SUT.Submodules.Gamma;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Tycho;
using Tycho.Contract.Inbox;
using Tycho.Contract.Outbox;
using Tycho.Structure;

namespace IntegrationTests.SendingMessagesHorizontally.SUT;

internal class AppModule : TychoModule
{
    protected override void DeclareOutgoingMessages(IOutboxDefinition outbox, IServiceProvider services)
    {
        outbox.Events.Declare<EventToSend>()
              .Events.Declare<MappedEvent>();

        outbox.Requests.Declare<RequestToSend>()
              .Requests.Declare<MappedRequest>();

        outbox.Requests.Declare<RequestWithResponseToSend, string>()
              .Requests.Declare<MappedRequestWithResponse, string>();
    }

    protected override void HandleIncomingMessages(IInboxDefinition inbox, IServiceProvider services)
    {
        inbox.Events.Handle<EventToSend, AppHandler>()
             .Events.Handle<EventToSendWithMapping, AppHandler>();

        inbox.Requests.Handle<RequestToSend, AppHandler>()
             .Requests.Handle<RequestToSendWithMapping, AppHandler>();

        inbox.Requests.Handle<RequestWithResponseToSend, string, AppHandler>()
             .Requests.Handle<RequestWithResponseToSendWithMapping, string, AppHandler>();
    }

    protected override void IncludeSubmodules(ISubstructureDefinition structure, IServiceProvider services)
    {
        structure.AddSubmodule<AlphaModule>(alphaOutbox =>
        {
            alphaOutbox.Events.Handle<EventToSend, AlphaHandler>()
                       .Events.Handle<AlphaOutEvent, AlphaHandler>();

            alphaOutbox.Requests.Handle<RequestToSend, AlphaHandler>()
                       .Requests.Handle<AlphaOutRequest, AlphaHandler>();

            alphaOutbox.Requests.Handle<RequestWithResponseToSend, string, AlphaHandler>()
                       .Requests.Handle<AlphaOutRequestWithResponse, AlphaOutRequestWithResponse.Response, AlphaHandler>();
        });

        structure.AddSubmodule<BetaModule>(alphaOutbox =>
        {
            alphaOutbox.Events.Handle<EventToSend, BetaHandler>()
                       .Events.Handle<BetaOutEvent, BetaHandler>();

            alphaOutbox.Requests.Handle<RequestToSend, BetaHandler>()
                       .Requests.Handle<BetaOutRequest, BetaHandler>();

            alphaOutbox.Requests.Handle<RequestWithResponseToSend, string, BetaHandler>()
                       .Requests.Handle<BetaOutRequestWithResponse, BetaOutRequestWithResponse.Response, BetaHandler>();
        });

        structure.AddSubmodule<GammaModule>(alphaOutbox =>
        {
            alphaOutbox.Events.Handle<EventToSend, GammaHandler>()
                       .Events.Handle<GammaOutEvent, GammaHandler>();

            alphaOutbox.Requests.Handle<RequestToSend, GammaHandler>()
                       .Requests.Handle<GammaOutRequest, GammaHandler>();

            alphaOutbox.Requests.Handle<RequestWithResponseToSend, string, GammaHandler>()
                       .Requests.Handle<GammaOutRequestWithResponse, GammaOutRequestWithResponse.Response, GammaHandler>();
        });
    }

    protected override void RegisterServices(IServiceCollection services, IConfiguration configuration) { }
}
