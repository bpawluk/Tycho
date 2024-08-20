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
internal record FromBetaEvent(string Id) : IEvent;
internal record FromBetaCommand(string Id) : IRequest;
internal record FromBetaQuery(string Id) : IRequest<string>;

// Outgoing
internal record GammaEvent(string Id) : IEvent;
internal record GammaCommand(string Id) : IRequest;
internal record GammaQuery(string Id) : IRequest<string>;

internal class GammaModule : TychoModule
{
    protected override void HandleIncomingMessages(IInboxDefinition module, IServiceProvider services)
    {
        var thisModule = services.GetRequiredService<IModule>();

        module.SubscribesTo<FromBetaEvent>(eventData =>
        {
            thisModule.Publish<GammaEvent>(new(eventData.Id));
        });

        module.Executes<FromBetaCommand>(commandData =>
        {
            thisModule.Execute<GammaCommand>(new(commandData.Id));
        });

        module.RespondsTo<FromBetaQuery, string>(queryData =>
        {
            return thisModule.Execute<GammaQuery, string>(new(queryData.Id));
        });
    }

    protected override void DeclareOutgoingMessages(IOutboxDefinition module, IServiceProvider services)
    {
        module.Publishes<GammaEvent>()
              .Sends<GammaCommand>()
              .Sends<GammaQuery, string>();
    }

    protected override void IncludeSubmodules(ISubstructureDefinition module, IServiceProvider services) { }

    protected override void RegisterServices(IServiceCollection services, IConfiguration configuration) { }
}
