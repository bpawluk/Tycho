using IntegrationTests.ServiceRegistrationAndResolving.SUT.Handlers;
using IntegrationTests.ServiceRegistrationAndResolving.SUT.Services;
using IntegrationTests.ServiceRegistrationAndResolving.SUT.Submodules;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Tycho;
using Tycho.Contract.Inbox;
using Tycho.Contract.Outbox;
using Tycho.Messaging.Payload;
using Tycho.Structure;

namespace IntegrationTests.ServiceRegistrationAndResolving.SUT;

// Incoming
internal record SingletonServiceWorkflowRequest : IRequest<int>;
internal record TransientServiceWorkflowRequest : IRequest<int>;
internal record SubmoduleResolvingWorkflowRequest : IRequest<string>;
internal record ModuleResolvingWorkflowRequest : IRequest<string>;

// Outgoing
internal record GetDataFromThisModulesClientRequestWithResponse() : IRequest<string>;

internal class AppModule : TychoModule
{
    protected override void HandleIncomingMessages(IInboxDefinition module, IServiceProvider services)
    {
        module.Requests.Handle<SingletonServiceWorkflowRequest, int, SingletonServiceWorkflowRequestHandler>()
              .Requests.Handle<TransientServiceWorkflowRequest, int, TransientServiceWorkflowRequestHandler>()
              .Requests.Handle<SubmoduleResolvingWorkflowRequest, string, SubmoduleResolvingWorkflowRequestHandler>()
              .Requests.Handle<ModuleResolvingWorkflowRequest, string, ModuleResolvingWorkflowRequestHandler>();
    }

    protected override void DeclareOutgoingMessages(IOutboxDefinition module, IServiceProvider services) 
    {
        module.Requests.Declare<GetDataFromThisModulesClientRequestWithResponse, string>();
    }

    protected override void IncludeSubmodules(ISubstructureDefinition module, IServiceProvider services)
    {
        module.AddSubmodule<AppSubmodule>();
    }

    protected override void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ISingletonService, SingletonService>()
                .AddTransient<ITransientService, TransientService>();
    }
}
