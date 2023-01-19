using Microsoft.Extensions.DependencyInjection;
using System;
using Test.Integration.PassingMessagesBetweenSubmodules.SUT.Handlers;
using Test.Integration.PassingMessagesBetweenSubmodules.SUT.Submodules;
using Tycho;
using Tycho.Contract;
using Tycho.Messaging.Payload;
using Tycho.Structure;

namespace Test.Integration.PassingMessagesBetweenSubmodules.SUT;

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
        var alphaSubmodule = services.GetRequiredService<ISubmodule<AlphaModule>>()!;
        module.Executes<BeginEventWorkflowCommand>(commandData => alphaSubmodule.ExecuteCommand(commandData))
              .Executes<BeginCommandWorkflowCommand>(commandData => alphaSubmodule.ExecuteCommand(commandData))
              .Executes<BeginQueryWorkflowCommand>(commandData => alphaSubmodule.ExecuteCommand(commandData));
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
                thisModule.PublishEvent<EventWorkflowCompletedEvent>(new(eventData.Id));
            });

            consumer.HandleCommand<GammaCommand>(commandData =>
            {
                thisModule.PublishEvent<CommandWorkflowCompletedEvent>(new(commandData.Id));
            });

            consumer.HandleQuery<GammaQuery, string>(queryData =>
            {
                thisModule.PublishEvent<QueryWorkflowCompletedEvent>(new(queryData.Id));
                return "Hello world!";
            });
        }));
    }

    protected override void RegisterServices(IServiceCollection services) { }
}
