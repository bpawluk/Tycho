using IntegrationTests.SendingMessagesDownTheHierarchy.SUT.Submodules;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Tycho;
using Tycho.Contract;
using Tycho.Messaging.Payload;
using Tycho.Structure;

namespace IntegrationTests.SendingMessagesDownTheHierarchy.SUT;

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
        var alphaModule = services.GetRequiredService<IModule<AlphaModule>>();

        module.Executes<BeginEventWorkflowCommand>(commandData =>
        {
            alphaModule.Publish<AlphaDownstreamEvent>(new(commandData.Id));
        });

        module.Executes<BeginCommandWorkflowCommand>(commandData =>
        {
            alphaModule.Execute<AlphaDownstreamCommand>(new(commandData.Id));
        });

        module.Executes<BeginQueryWorkflowCommand>(commandData =>
        {
            alphaModule.Execute<AlphaDownstreamQuery, string>(new(commandData.Id));
        });
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
            var thisModule = services.GetRequiredService<IModule>();

            consumer.HandleEvent<AlphaUpstreamEvent>(eventData =>
            {
                thisModule.Publish<EventWorkflowCompletedEvent>(new(eventData.Id));
            });

            consumer.HandleCommand<AlphaUpstreamCommand>(commandData =>
            {
                thisModule.Publish<CommandWorkflowCompletedEvent>(new(commandData.Id));
            });

            consumer.HandleQuery<AlphaUpstreamQuery, string>(queryData =>
            {
                thisModule.Publish<QueryWorkflowCompletedEvent>(new(queryData.Id));
                return "Hello world!";
            });
        });
    }

    protected override void RegisterServices(IServiceCollection services, IConfiguration configuration) { }
}
