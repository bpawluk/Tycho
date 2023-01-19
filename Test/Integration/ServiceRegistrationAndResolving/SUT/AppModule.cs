using Microsoft.Extensions.DependencyInjection;
using System;
using Test.Integration.ServiceRegistrationAndResolving.SUT.Handlers;
using Test.Integration.ServiceRegistrationAndResolving.SUT.Services;
using Test.Integration.ServiceRegistrationAndResolving.SUT.Submodules;
using Tycho;
using Tycho.Contract;
using Tycho.Messaging.Payload;
using Tycho.Structure;

namespace Test.Integration.ServiceRegistrationAndResolving.SUT;

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

    protected override void RegisterServices(IServiceCollection services)
    {
        services.AddSingleton<ISingletonService, SingletonService>()
                .AddTransient<ITransientService, TransientService>();
    }
}
