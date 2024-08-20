using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Tycho;
using Tycho.Contract.Inbox;
using Tycho.Contract.Outbox;
using Tycho.Messaging.Payload;
using Tycho.Structure;

namespace IntegrationTests.PassingMessagesBetweenSubmodules.SUT.Submodules;

// Incoming
internal record BeginEventWorkflowRequest(string Id) : IRequest;
internal record BeginRequestWorkflowRequest(string Id) : IRequest;
internal record BeginRequestWithResponseWorkflowRequest(string Id) : IRequest;

// Outgoing
internal record AlphaEvent(string Id) : IEvent;
internal record AlphaRequest(string Id) : IRequest;
internal record AlphaRequestWithResponse(string Id) : IRequest<string>;

internal class AlphaModule : TychoModule
{
    protected override void HandleIncomingMessages(IInboxDefinition module, IServiceProvider services)
    {
        var thisModule = services.GetRequiredService<IModule>();

        module.Requests.Handle<BeginEventWorkflowRequest>(requestData =>
        {
            thisModule.Publish<AlphaEvent>(new(requestData.Id));
        });

        module.Requests.Handle<BeginRequestWorkflowRequest>(requestData =>
        {
            thisModule.Execute<AlphaRequest>(new(requestData.Id));
        });

        module.Requests.Handle<BeginRequestWithResponseWorkflowRequest>(requestData =>
        {
            thisModule.Execute<AlphaRequestWithResponse, string>(new(requestData.Id));
        });
    }

    protected override void DeclareOutgoingMessages(IOutboxDefinition module, IServiceProvider services)
    {
        module.Events.Declare<AlphaEvent>()
              .Requests.Declare<AlphaRequest>()
              .Requests.Declare<AlphaRequestWithResponse, string>();
    }

    protected override void IncludeSubmodules(ISubstructureDefinition module, IServiceProvider services) { }

    protected override void RegisterServices(IServiceCollection services, IConfiguration configuration) { }
}
