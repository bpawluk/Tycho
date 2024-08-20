using IntegrationTests.ProvidingConfiguration.SUT.Configuration;
using IntegrationTests.ProvidingConfiguration.SUT.Handlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Tycho;
using Tycho.Contract.Inbox;
using Tycho.Contract.Outbox;
using Tycho.Messaging.Payload;
using Tycho.Structure;

namespace IntegrationTests.ProvidingConfiguration.SUT;

// Incoming
internal record GetConfiguredValueViaBindingQuery() : IRequest<int>;
internal record GetConfiguredValueViaIConfigurationQuery() : IRequest<DateTime>;

// Outgoing
// - no incoming messages specific to this module

internal class AppModule : TychoModule
{
    protected override void HandleIncomingMessages(IInboxDefinition module, IServiceProvider services)
    {
        module.RespondsTo<GetConfiguredValueViaBindingQuery, int, GetConfiguredValueViaBindingQueryHandler>()
              .RespondsTo<GetConfiguredValueViaIConfigurationQuery, DateTime, GetConfiguredValueViaIConfigurationQueryHandler>();
    }

    protected override void DeclareOutgoingMessages(IOutboxDefinition module, IServiceProvider services) { }

    protected override void IncludeSubmodules(ISubstructureDefinition module, IServiceProvider services) { }

    protected override void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(configuration.Get<TestConfig>()!);
    }
}
