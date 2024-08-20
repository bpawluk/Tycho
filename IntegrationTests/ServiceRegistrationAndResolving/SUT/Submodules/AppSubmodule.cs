using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Tycho;
using Tycho.Contract.Inbox;
using Tycho.Contract.Outbox;
using Tycho.Messaging.Payload;
using Tycho.Structure;

namespace IntegrationTests.ServiceRegistrationAndResolving.SUT.Submodules;

// Incoming
internal record GetDataFromSubmoduleRequestWithResponse() : IRequest<string>;

// Outgoing
// - no outgoing messages specific to this module


internal class AppSubmodule : TychoModule
{
    protected override void HandleIncomingMessages(IInboxDefinition module, IServiceProvider services) 
    {
        module.Requests.Handle<GetDataFromSubmoduleRequestWithResponse, string>(_ => $"Response from {typeof(AppSubmodule).Name}");
    }

    protected override void DeclareOutgoingMessages(IOutboxDefinition module, IServiceProvider services) { }

    protected override void IncludeSubmodules(ISubstructureDefinition module, IServiceProvider services) { }

    protected override void RegisterServices(IServiceCollection services, IConfiguration configuration) { }
}
