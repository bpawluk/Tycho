﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Tycho;
using Tycho.Contract.Inbox;
using Tycho.Contract.Outbox;
using Tycho.Messaging.Payload;
using Tycho.Structure;

namespace IntegrationTests.PassingMessagesBetweenSubmodules.SUT.Submodules;

// Incoming
internal record FromAlphaEvent(string Id) : IEvent;
internal record FromAlphaCommand(string Id) : IRequest;
internal record FromAlphaQuery(string Id) : IRequest<string>;

// Outgoing
internal record BetaEvent(string Id) : IEvent;
internal record BetaCommand(string Id) : IRequest;
internal record BetaQuery(string Id) : IRequest<string>;

internal class BetaModule : TychoModule
{
    protected override void HandleIncomingMessages(IInboxDefinition module, IServiceProvider services)
    {
        var thisModule = services.GetRequiredService<IModule>();

        module.SubscribesTo<FromAlphaEvent>(eventData =>
        {
            thisModule.Publish<BetaEvent>(new(eventData.Id));
        });

        module.Executes<FromAlphaCommand>(commandData =>
        {
            thisModule.Execute<BetaCommand>(new(commandData.Id));
        });

        module.RespondsTo<FromAlphaQuery, string>(queryData =>
        {
            return thisModule.Execute<BetaQuery, string>(new(queryData.Id));
        });
    }

    protected override void DeclareOutgoingMessages(IOutboxDefinition module, IServiceProvider services)
    {
        module.Publishes<BetaEvent>()
              .Sends<BetaCommand>()
              .Sends<BetaQuery, string>();
    }

    protected override void IncludeSubmodules(ISubstructureDefinition module, IServiceProvider services) { }

    protected override void RegisterServices(IServiceCollection services, IConfiguration configuration) { }
}
