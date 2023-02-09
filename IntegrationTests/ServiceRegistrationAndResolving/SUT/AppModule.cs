using IntegrationTests.ServiceRegistrationAndResolving.SUT.Handlers;
using IntegrationTests.ServiceRegistrationAndResolving.SUT.Services;
using IntegrationTests.ServiceRegistrationAndResolving.SUT.Submodules;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Tycho;
using Tycho.Contract;
using Tycho.Messaging.Payload;
using Tycho.Structure;

namespace IntegrationTests.ServiceRegistrationAndResolving.SUT;

// Incoming
internal record SingletonServiceWorkflowQuery : IQuery<int>;
internal record TransientServiceWorkflowQuery : IQuery<int>;
internal record SubmoduleResolvingWorkflowQuery : IQuery<string>;
internal record ModuleResolvingWorkflowQuery : IQuery<string>;

// Outgoing
internal record GetDataFromThisModulesClientQuery() : IQuery<string>;

internal class AppModule : TychoModule
{
    protected override void DeclareIncomingMessages(IInboxDefinition module, IServiceProvider services)
    {
        module.RespondsTo<SingletonServiceWorkflowQuery, int, SingletonServiceWorkflowQueryHandler>()
              .RespondsTo<TransientServiceWorkflowQuery, int, TransientServiceWorkflowQueryHandler>()
              .RespondsTo<SubmoduleResolvingWorkflowQuery, string, SubmoduleResolvingWorkflowQueryHandler>()
              .RespondsTo<ModuleResolvingWorkflowQuery, string, ModuleResolvingWorkflowQueryHandler>();
    }

    protected override void DeclareOutgoingMessages(IOutboxDefinition module, IServiceProvider services) 
    {
        module.Sends<GetDataFromThisModulesClientQuery, string>();
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
