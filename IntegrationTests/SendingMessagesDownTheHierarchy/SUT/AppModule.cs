using IntegrationTests.SendingMessagesDownTheHierarchy.SUT.Submodules;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Tycho;
using Tycho.Contract.Inbox;
using Tycho.Contract.Outbox;
using Tycho.Messaging.Payload;
using Tycho.Structure;

namespace IntegrationTests.SendingMessagesDownTheHierarchy.SUT;

// Incoming
internal record BeginEventWorkflowCommand(string Id) : IRequest;
internal record BeginCommandWorkflowCommand(string Id) : IRequest;
internal record BeginQueryWorkflowCommand(string Id) : IRequest;

// Outgoing
internal record EventWorkflowCompletedEvent(string Id) : IEvent;
internal record CommandWorkflowCompletedEvent(string Id) : IEvent;
internal record QueryWorkflowCompletedEvent(string Id) : IEvent;

internal class AppModule : TychoModule
{
    protected override void HandleIncomingMessages(IInboxDefinition module, IServiceProvider services)
    {
        var alphaModule = services.GetRequiredService<IModule<AlphaModule>>();

        module.Requests.Handle<BeginEventWorkflowCommand>(commandData =>
        {
            alphaModule.Publish<AlphaDownstreamEvent>(new(commandData.Id));
        });

        module.Requests.Handle<BeginCommandWorkflowCommand>(commandData =>
        {
            alphaModule.Execute<AlphaDownstreamCommand>(new(commandData.Id));
        });

        module.Requests.Handle<BeginQueryWorkflowCommand>(commandData =>
        {
            alphaModule.Execute<AlphaDownstreamQuery, string>(new(commandData.Id));
        });
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
            var thisModule = services.GetRequiredService<IModule>();

            consumer.Events.Handle<AlphaUpstreamEvent>(eventData =>
            {
                thisModule.Publish<EventWorkflowCompletedEvent>(new(eventData.Id));
            });

            consumer.Requests.Handle<AlphaUpstreamCommand>(commandData =>
            {
                thisModule.Publish<CommandWorkflowCompletedEvent>(new(commandData.Id));
            });

            consumer.Requests.Handle<AlphaUpstreamQuery, string>(queryData =>
            {
                thisModule.Publish<QueryWorkflowCompletedEvent>(new(queryData.Id));
                return "Hello world!";
            });
        });
    }

    protected override void RegisterServices(IServiceCollection services, IConfiguration configuration) { }
}
