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
internal record BeginEventWorkflowCommand(string Id) : IRequest;
internal record BeginCommandWorkflowCommand(string Id) : IRequest;
internal record BeginQueryWorkflowCommand(string Id) : IRequest;

// Outgoing
internal record AlphaEvent(string Id) : IEvent;
internal record AlphaCommand(string Id) : IRequest;
internal record AlphaQuery(string Id) : IRequest<string>;

internal class AlphaModule : TychoModule
{
    protected override void HandleIncomingMessages(IInboxDefinition module, IServiceProvider services)
    {
        var thisModule = services.GetRequiredService<IModule>();

        module.Requests.Handle<BeginEventWorkflowCommand>(commandData =>
        {
            thisModule.Publish<AlphaEvent>(new(commandData.Id));
        });

        module.Requests.Handle<BeginCommandWorkflowCommand>(commandData =>
        {
            thisModule.Execute<AlphaCommand>(new(commandData.Id));
        });

        module.Requests.Handle<BeginQueryWorkflowCommand>(commandData =>
        {
            thisModule.Execute<AlphaQuery, string>(new(commandData.Id));
        });
    }

    protected override void DeclareOutgoingMessages(IOutboxDefinition module, IServiceProvider services)
    {
        module.Events.Declare<AlphaEvent>()
              .Requests.Declare<AlphaCommand>()
              .Requests.Declare<AlphaQuery, string>();
    }

    protected override void IncludeSubmodules(ISubstructureDefinition module, IServiceProvider services) { }

    protected override void RegisterServices(IServiceCollection services, IConfiguration configuration) { }
}
