using IntegrationTests.DefiningGenericModules.SUT.Handlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Tycho;
using Tycho.Contract.Inbox;
using Tycho.Contract.Outbox;
using Tycho.Messaging.Payload;
using Tycho.Structure;

namespace IntegrationTests.DefiningGenericModules.SUT;

// Incoming
internal record GenericRequestWithResponse<T>(T Data) : IRequest<T>;

// Outgoing
// - no incoming messages specific to this module

internal class GenericModule<T> : TychoModule
{
    protected override void HandleIncomingMessages(IInboxDefinition module, IServiceProvider services)
    {
        module.Requests.Handle<GenericRequestWithResponse<T>, T, GenericRequestWithResponseHandler<T>>();
    }

    protected override void DeclareOutgoingMessages(IOutboxDefinition module, IServiceProvider services) { }

    protected override void IncludeSubmodules(ISubstructureDefinition module, IServiceProvider services) { }

    protected override void RegisterServices(IServiceCollection services, IConfiguration configuration) { }
}
