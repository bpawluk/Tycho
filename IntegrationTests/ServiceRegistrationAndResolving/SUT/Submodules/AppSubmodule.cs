using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho;
using Tycho.Contract.Inbox;
using Tycho.Contract.Outbox;
using Tycho.Messaging.Handlers;
using Tycho.Messaging.Payload;
using Tycho.Structure;

namespace IntegrationTests.ServiceRegistrationAndResolving.SUT.Submodules;

// Incoming
internal record GetDataFromSubmoduleRequest() : IRequest<string>;

// Outgoing
// - no outgoing messages specific to this module

internal class AppSubmodule : TychoModule
{
    protected override void HandleIncomingMessages(IInboxDefinition module, IServiceProvider services) 
    {
        module.Requests.Handle<GetDataFromSubmoduleRequest, string, GetDataFromSubmoduleRequestHandler>();
    }

    protected override void DeclareOutgoingMessages(IOutboxDefinition module, IServiceProvider services) { }

    protected override void IncludeSubmodules(ISubstructureDefinition module, IServiceProvider services) { }

    protected override void RegisterServices(IServiceCollection services, IConfiguration configuration) { }
}

internal class GetDataFromSubmoduleRequestHandler : IRequestHandler<GetDataFromSubmoduleRequest, string>
{
    public Task<string> Handle(GetDataFromSubmoduleRequest _, CancellationToken cancellationToken) => 
        Task.FromResult($"Response from {typeof(AppSubmodule).Name}");
}
