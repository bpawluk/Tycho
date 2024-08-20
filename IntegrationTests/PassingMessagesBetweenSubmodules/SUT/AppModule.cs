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
internal record CommandWorkflowCompletedEvent(string Id) : IEvent;
internal record QueryWorkflowCompletedEvent(string Id) : IEvent;

internal class AppModule : TychoModule
{
    protected override void HandleIncomingMessages(IInboxDefinition module, IServiceProvider services)
    {
        var alphaSubmodule = services.GetRequiredService<IModule<AlphaModule>>()!;
        module.Requests.Handle<BeginEventWorkflowCommand>(commandData => alphaSubmodule.Execute(commandData))
              .Requests.Handle<BeginCommandWorkflowCommand>(commandData => alphaSubmodule.Execute(commandData))
              .Requests.Handle<BeginQueryWorkflowCommand>(commandData => alphaSubmodule.Execute(commandData));
    }

    protected override void DeclareOutgoingMessages(IOutboxDefinition module, IServiceProvider services)
    {
        module.Events.Declare<EventWorkflowCompletedEvent>()
              .Events.Declare<CommandWorkflowCompletedEvent>()
              .Events.Declare<QueryWorkflowCompletedEvent>();
    }

    protected override void IncludeSubmodules(ISubstructureDefinition module, IServiceProvider services)
    {
        module.AddSubmodule<AlphaModule>(consumer =>
        {
            consumer.Events.Handle<AlphaEvent, AlphaBetaProxyHandler>()
                    .Requests.Handle<AlphaCommand, AlphaBetaProxyHandler>()
                    .Requests.Handle<AlphaQuery, string, AlphaBetaProxyHandler>();
        });

        module.AddSubmodule<BetaModule>(consumer =>
        {
            consumer.Events.Handle<BetaEvent, BetaGammaProxyHandler>()
                    .Requests.Handle<BetaCommand, BetaGammaProxyHandler>()
                    .Requests.Handle<BetaQuery, string, BetaGammaProxyHandler>();
        });

        module.AddSubmodule<GammaModule>((consumer =>
        {
            var thisModule = services.GetService<IModule>()!;

            consumer.Events.Handle<GammaEvent>(eventData =>
            {
                thisModule.Publish<EventWorkflowCompletedEvent>(new(eventData.Id));
            });

            consumer.Requests.Handle<GammaCommand>(commandData =>
            {
                thisModule.Publish<CommandWorkflowCompletedEvent>(new(commandData.Id));
            });

            consumer.Requests.Handle<GammaQuery, string>(queryData =>
            {
                thisModule.Publish<QueryWorkflowCompletedEvent>(new(queryData.Id));
                return "Hello world!";
            });
        }));
    }

    protected override void RegisterServices(IServiceCollection services, IConfiguration configuration) { }
}
