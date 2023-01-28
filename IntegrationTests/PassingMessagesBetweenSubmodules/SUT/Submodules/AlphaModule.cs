using Microsoft.Extensions.DependencyInjection;
using System;
using Tycho;
using Tycho.Contract;
using Tycho.Messaging.Payload;
using Tycho.Structure;

namespace IntegrationTests.PassingMessagesBetweenSubmodules.SUT.Submodules;

// Incoming
internal record BeginEventWorkflowCommand(string Id) : ICommand;
internal record BeginCommandWorkflowCommand(string Id) : ICommand;
internal record BeginQueryWorkflowCommand(string Id) : ICommand;

// Outgoing
internal record AlphaEvent(string Id) : IEvent;
internal record AlphaCommand(string Id) : ICommand;
internal record AlphaQuery(string Id) : IQuery<string>;

internal class AlphaModule : TychoModule
{
    protected override void DeclareIncomingMessages(IInboxDefinition module, IServiceProvider services)
    {
        var thisModule = services.GetRequiredService<IModule>();

        module.Executes<BeginEventWorkflowCommand>(commandData =>
        {
            thisModule.Publish<AlphaEvent>(new(commandData.Id));
        });

        module.Executes<BeginCommandWorkflowCommand>(commandData =>
        {
            thisModule.Execute<AlphaCommand>(new(commandData.Id));
        });

        module.Executes<BeginQueryWorkflowCommand>(commandData =>
        {
            thisModule.Execute<AlphaQuery, string>(new(commandData.Id));
        });
    }

    protected override void DeclareOutgoingMessages(IOutboxDefinition module, IServiceProvider services)
    {
        module.Publishes<AlphaEvent>()
              .Sends<AlphaCommand>()
              .Sends<AlphaQuery, string>();
    }

    protected override void IncludeSubmodules(ISubstructureDefinition module, IServiceProvider services) { }

    protected override void RegisterServices(IServiceCollection services) { }
}
