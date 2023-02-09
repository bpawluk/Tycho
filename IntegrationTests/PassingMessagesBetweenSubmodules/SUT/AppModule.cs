using IntegrationTests.PassingMessagesBetweenSubmodules.SUT.Handlers;
using IntegrationTests.PassingMessagesBetweenSubmodules.SUT.Submodules;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Tycho;
using Tycho.Contract;
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
    protected override void DeclareIncomingMessages(IInboxDefinition module, IServiceProvider services)
    {
        var alphaSubmodule = services.GetRequiredService<IModule<AlphaModule>>()!;
        module.Executes<BeginEventWorkflowCommand>(commandData => alphaSubmodule.Execute(commandData))
              .Executes<BeginCommandWorkflowCommand>(commandData => alphaSubmodule.Execute(commandData))
              .Executes<BeginQueryWorkflowCommand>(commandData => alphaSubmodule.Execute(commandData));
    }

    protected override void DeclareOutgoingMessages(IOutboxDefinition module, IServiceProvider services)
    {
        module.Publishes<EventWorkflowCompletedEvent>()
              .Publishes<CommandWorkflowCompletedEvent>()
              .Publishes<QueryWorkflowCompletedEvent>();
    }

    protected override void IncludeSubmodules(ISubstructureDefinition module, IServiceProvider services)
    {
        module.AddSubmodule<AlphaModule>(consumer =>
        {
            consumer.HandleEvent<AlphaEvent, AlphaBetaProxyHandler>()
                    .HandleCommand<AlphaCommand, AlphaBetaProxyHandler>()
                    .HandleQuery<AlphaQuery, string, AlphaBetaProxyHandler>();
        });

        module.AddSubmodule<BetaModule>(consumer =>
        {
            consumer.HandleEvent<BetaEvent, BetaGammaProxyHandler>()
                    .HandleCommand<BetaCommand, BetaGammaProxyHandler>()
                    .HandleQuery<BetaQuery, string, BetaGammaProxyHandler>();
        });

        module.AddSubmodule<GammaModule>((consumer =>
        {
            var thisModule = services.GetService<IModule>()!;

            consumer.HandleEvent<GammaEvent>(eventData =>
            {
                thisModule.Publish<EventWorkflowCompletedEvent>(new(eventData.Id));
            });

            consumer.HandleCommand<GammaCommand>(commandData =>
            {
                thisModule.Publish<CommandWorkflowCompletedEvent>(new(commandData.Id));
            });

            consumer.HandleQuery<GammaQuery, string>(queryData =>
            {
                thisModule.Publish<QueryWorkflowCompletedEvent>(new(queryData.Id));
                return "Hello world!";
            });
        }));
    }

    protected override void RegisterServices(IServiceCollection services, IConfiguration configuration) { }
}
