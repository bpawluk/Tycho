﻿using Microsoft.Extensions.DependencyInjection;
using System;
using Tycho;
using Tycho.Contract;
using Tycho.Messaging.Payload;
using Tycho.Structure;

namespace Test.Integration.PassingMessagesBetweenSubmodules.SUT.Submodules;

// Incoming
internal record FromBetaEvent(string Id) : IEvent;
internal record FromBetaCommand(string Id) : ICommand;
internal record FromBetaQuery(string Id) : IQuery<string>;

// Outgoing
internal record GammaEvent(string Id) : IEvent;
internal record GammaCommand(string Id) : ICommand;
internal record GammaQuery(string Id) : IQuery<string>;

internal class GammaModule : TychoModule
{
    protected override void DeclareIncomingMessages(IInboxDefinition module, IServiceProvider services)
    {
        var thisModule = services.GetRequiredService<IModule>();

        module.SubscribesTo<FromBetaEvent>(eventData =>
        {
            thisModule.PublishEvent<GammaEvent>(new(eventData.Id));
        });

        module.Executes<FromBetaCommand>(commandData =>
        {
            thisModule.ExecuteCommand<GammaCommand>(new(commandData.Id));
        });

        module.RespondsTo<FromBetaQuery, string>(queryData =>
        {
            return thisModule.ExecuteQuery<GammaQuery, string>(new(queryData.Id));
        });
    }

    protected override void DeclareOutgoingMessages(IOutboxDefinition module, IServiceProvider services)
    {
        module.Publishes<GammaEvent>()
              .Sends<GammaCommand>()
              .Sends<GammaQuery, string>();
    }

    protected override void IncludeSubmodules(ISubstructureDefinition module, IServiceProvider services) { }

    protected override void RegisterServices(IServiceCollection services) { }
}
