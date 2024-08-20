using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Tycho;
using Tycho.Contract.Inbox;
using Tycho.Contract.Outbox;
using Tycho.Messaging.Payload;
using Tycho.Structure;

namespace IntegrationTests.SendingMessagesDownTheHierarchy.SUT.Submodules;

// Incoming
internal record GammaDownstreamEvent(string Id) : IEvent;
internal record GammaDownstreamCommand(string Id) : IRequest;
internal record GammaDownstreamQuery(string Id) : IRequest<string>;

// Outgoing
internal record GammaUpstreamEvent(string Id) : IEvent;
internal record GammaUpstreamCommand(string Id) : IRequest;
internal record GammaUpstreamQuery(string Id) : IRequest<string>;

internal class GammaModule : TychoModule
{
    protected override void HandleIncomingMessages(IInboxDefinition module, IServiceProvider services)
    {
        var thisModule = services.GetRequiredService<IModule>();

        module.Events.Handle<GammaDownstreamEvent>(eventData =>
        {
            thisModule.Publish<GammaUpstreamEvent>(new(eventData.Id));
        });

        module.Requests.Handle<GammaDownstreamCommand>(commandData =>
        {
            thisModule.Execute<GammaUpstreamCommand>(new(commandData.Id));
        });

        module.Requests.Handle<GammaDownstreamQuery, string>(queryData =>
        {
            return thisModule.Execute<GammaUpstreamQuery, string>(new(queryData.Id));
        });
    }

    protected override void DeclareOutgoingMessages(IOutboxDefinition module, IServiceProvider services)
    {
        module.Events.Declare<GammaUpstreamEvent>()
              .Requests.Declare<GammaUpstreamCommand>()
              .Requests.Declare<GammaUpstreamQuery, string>();
    }

    protected override void IncludeSubmodules(ISubstructureDefinition module, IServiceProvider services) { }

    protected override void RegisterServices(IServiceCollection services, IConfiguration configuration) { }
}
