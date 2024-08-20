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
internal record BeginEventWorkflowRequest(string Id) : IRequest;
internal record BeginRequestWorkflowRequest(string Id) : IRequest;
internal record BeginRequestWithResponseWorkflowRequest(string Id) : IRequest;

// Outgoing
internal record EventWorkflowCompletedEvent(string Id) : IEvent;
internal record RequestWorkflowCompletedEvent(string Id) : IEvent;
internal record RequestWithResponseWorkflowCompletedEvent(string Id) : IEvent;

internal class AppModule : TychoModule
{
    protected override void HandleIncomingMessages(IInboxDefinition module, IServiceProvider services)
    {
        var alphaModule = services.GetRequiredService<IModule<AlphaModule>>();

        module.Requests.Handle<BeginEventWorkflowRequest>(requestData =>
        {
            alphaModule.Publish<AlphaDownstreamEvent>(new(requestData.Id));
        });

        module.Requests.Handle<BeginRequestWorkflowRequest>(requestData =>
        {
            alphaModule.Execute<AlphaDownstreamRequest>(new(requestData.Id));
        });

        module.Requests.Handle<BeginRequestWithResponseWorkflowRequest>(requestData =>
        {
            alphaModule.Execute<AlphaDownstreamRequestWithResponse, string>(new(requestData.Id));
        });
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
            var thisModule = services.GetRequiredService<IModule>();

            consumer.Events.Handle<AlphaUpstreamEvent>(eventData =>
            {
                thisModule.Publish<EventWorkflowCompletedEvent>(new(eventData.Id));
            });

            consumer.Requests.Handle<AlphaUpstreamRequest>(requestData =>
            {
                thisModule.Publish<RequestWorkflowCompletedEvent>(new(requestData.Id));
            });

            consumer.Requests.Handle<AlphaUpstreamRequestWithResponse, string>(requestData =>
            {
                thisModule.Publish<RequestWithResponseWorkflowCompletedEvent>(new(requestData.Id));
                return "Hello world!";
            });
        });
    }

    protected override void RegisterServices(IServiceCollection services, IConfiguration configuration) { }
}
