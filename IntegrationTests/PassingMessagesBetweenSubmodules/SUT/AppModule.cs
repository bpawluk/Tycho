using IntegrationTests.PassingMessagesBetweenSubmodules.SUT.Handlers;
using IntegrationTests.PassingMessagesBetweenSubmodules.SUT.Submodules;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Tycho;
using Tycho.Contract.Inbox;
using Tycho.Contract.Outbox;
using Tycho.Messaging.Payload;
using Tycho.Structure;

namespace IntegrationTests.PassingMessagesBetweenSubmodules.SUT;

// Incoming
// - no incoming messages specific to this module

// Outgoing
internal record EventWorkflowCompletedEvent(string Id) : IEvent;
internal record RequestWorkflowCompletedEvent(string Id) : IEvent;
internal record RequestWithResponseWorkflowCompletedEvent(string Id) : IEvent;

internal class AppModule : TychoModule
{
    protected override void HandleIncomingMessages(IInboxDefinition module, IServiceProvider services)
    {
        var alphaSubmodule = services.GetRequiredService<IModule<AlphaModule>>()!;
        module.Requests.Handle<BeginEventWorkflowRequest>(requestData => alphaSubmodule.Execute(requestData))
              .Requests.Handle<BeginRequestWorkflowRequest>(requestData => alphaSubmodule.Execute(requestData))
              .Requests.Handle<BeginRequestWithResponseWorkflowRequest>(requestData => alphaSubmodule.Execute(requestData));
    }

    protected override void DeclareOutgoingMessages(IOutboxDefinition module, IServiceProvider services)
    {
        module.Events.Declare<EventWorkflowCompletedEvent>()
              .Events.Declare<RequestWorkflowCompletedEvent>()
              .Events.Declare<RequestWithResponseWorkflowCompletedEvent>();
    }

    protected override void IncludeSubmodules(ISubstructureDefinition module, IServiceProvider services)
    {
        module.AddSubmodule<AlphaModule>(consumer =>
        {
            consumer.Events.Handle<AlphaEvent, AlphaBetaProxyHandler>()
                    .Requests.Handle<AlphaRequest, AlphaBetaProxyHandler>()
                    .Requests.Handle<AlphaRequestWithResponse, string, AlphaBetaProxyHandler>();
        });

        module.AddSubmodule<BetaModule>(consumer =>
        {
            consumer.Events.Handle<BetaEvent, BetaGammaProxyHandler>()
                    .Requests.Handle<BetaRequest, BetaGammaProxyHandler>()
                    .Requests.Handle<BetaRequestWithResponse, string, BetaGammaProxyHandler>();
        });

        module.AddSubmodule<GammaModule>((consumer =>
        {
            var thisModule = services.GetService<IModule>()!;

            consumer.Events.Handle<GammaEvent>(eventData =>
            {
                thisModule.Publish<EventWorkflowCompletedEvent>(new(eventData.Id));
            });

            consumer.Requests.Handle<GammaRequest>(requestData =>
            {
                thisModule.Publish<RequestWorkflowCompletedEvent>(new(requestData.Id));
            });

            consumer.Requests.Handle<GammaRequestWithResponse, string>(requestData =>
            {
                thisModule.Publish<RequestWithResponseWorkflowCompletedEvent>(new(requestData.Id));
                return "Hello world!";
            });
        }));
    }

    protected override void RegisterServices(IServiceCollection services, IConfiguration configuration) { }
}
