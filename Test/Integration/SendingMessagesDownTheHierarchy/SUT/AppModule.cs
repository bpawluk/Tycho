using Microsoft.Extensions.DependencyInjection;
using System;
using Test.Integration.SendingMessagesDownTheHierarchy.SUT.Submodules;
using Tycho;
using Tycho.Contract;
using Tycho.Messaging.Payload;
using Tycho.Structure;

namespace Test.Integration.SendingMessagesDownTheHierarchy.SUT;

// Incoming
internal record BeginEventWorkflowCommand(string Id) : ICommand;
internal record BeginCommandWorkflowCommand(string Id) : ICommand;
internal record BeginQueryWorkflowCommand(string Id) : ICommand;

// Outgoing
internal record EventWorkflowCompletedEvent(string Id) : IEvent;
internal record CommandWorkflowCompletedEvent(string Id) : IEvent;
internal record QueryWorkflowCompletedEvent(string Id) : IEvent;

internal class AppModule : TychoModule
{
    protected override void DeclareIncomingMessages(IInboxDefinition module, IServiceProvider services)
    {
        var alphaModule = services.GetRequiredService<ISubmodule<AlphaModule>>();
        module.Executes<BeginEventWorkflowCommand>(commandData => alphaModule.PublishEvent<AlphaDownstreamEvent>(new(commandData.Id)));
        module.Executes<BeginCommandWorkflowCommand>(commandData => alphaModule.ExecuteCommand<AlphaDownstreamCommand>(new(commandData.Id)));
        module.Executes<BeginQueryWorkflowCommand>(commandData => alphaModule.ExecuteQuery<AlphaDownstreamQuery, string>(new(commandData.Id)));
    }

    protected override void DeclareOutgoingMessages(IOutboxDefinition module, IServiceProvider services)
    {
        module.Publishes<EventWorkflowCompletedEvent>();
        module.Publishes<CommandWorkflowCompletedEvent>();
        module.Publishes<QueryWorkflowCompletedEvent>();
    }

    protected override void IncludeSubmodules(ISubstructureDefinition module, IServiceProvider services)
    {
        module.AddSubmodule<AlphaModule>(consumer =>
        {
            var thisModule = services.GetRequiredService<IModule>();
            consumer.HandleEvent<AlphaUpstreamEvent>(eventData => thisModule.PublishEvent<EventWorkflowCompletedEvent>(new(eventData.Id)));
            consumer.HandleCommand<AlphaUpstreamCommand>(commandData => thisModule.PublishEvent<CommandWorkflowCompletedEvent>(new(commandData.Id)));
            consumer.HandleQuery<AlphaUpstreamQuery, string>(queryData =>
            {
                thisModule.PublishEvent<QueryWorkflowCompletedEvent>(new(queryData.Id));
                return "Hello world!";
            });
        });
    }

    protected override void RegisterServices(IServiceCollection services) { }
}
