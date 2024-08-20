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
internal record SingletonServiceWorkflowQuery : IRequest<int>;
internal record TransientServiceWorkflowQuery : IRequest<int>;
internal record SubmoduleResolvingWorkflowQuery : IRequest<string>;
internal record ModuleResolvingWorkflowQuery : IRequest<string>;

// Outgoing
internal record GetDataFromThisModulesClientQuery() : IRequest<string>;

internal class AppModule : TychoModule
{
    protected override void HandleIncomingMessages(IInboxDefinition module, IServiceProvider services)
    {
        module.Requests.Handle<SingletonServiceWorkflowQuery, int, SingletonServiceWorkflowQueryHandler>()
              .Requests.Handle<TransientServiceWorkflowQuery, int, TransientServiceWorkflowQueryHandler>()
              .Requests.Handle<SubmoduleResolvingWorkflowQuery, string, SubmoduleResolvingWorkflowQueryHandler>()
              .Requests.Handle<ModuleResolvingWorkflowQuery, string, ModuleResolvingWorkflowQueryHandler>();
    }

    protected override void DeclareOutgoingMessages(IOutboxDefinition module, IServiceProvider services) 
    {
        module.Requests.Declare<GetDataFromThisModulesClientQuery, string>();
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
