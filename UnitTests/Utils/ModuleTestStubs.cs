using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Tycho;
using Tycho.Contract.Inbox;
using Tycho.Contract.Outbox;
using Tycho.Structure;

namespace UnitTests.Utils
{
    public class TestModule : TychoModule
    {
        protected override void HandleIncomingMessages(IInboxDefinition module, IServiceProvider services) { }
        protected override void DeclareOutgoingMessages(IOutboxDefinition module, IServiceProvider services) { }
        protected override void IncludeSubmodules(ISubstructureDefinition module, IServiceProvider services) { }
        protected override void RegisterServices(IServiceCollection services, IConfiguration configuration) { }
    }

    public class OtherModule : TychoModule
    {
        protected override void HandleIncomingMessages(IInboxDefinition module, IServiceProvider services) { }
        protected override void DeclareOutgoingMessages(IOutboxDefinition module, IServiceProvider services) { }
        protected override void IncludeSubmodules(ISubstructureDefinition module, IServiceProvider services) { }
        protected override void RegisterServices(IServiceCollection services, IConfiguration configuration) { }
    }

    public class YetAnotherModule : TychoModule
    {
        protected override void HandleIncomingMessages(IInboxDefinition module, IServiceProvider services) { }
        protected override void DeclareOutgoingMessages(IOutboxDefinition module, IServiceProvider services) { }
        protected override void IncludeSubmodules(ISubstructureDefinition module, IServiceProvider services) { }
        protected override void RegisterServices(IServiceCollection services, IConfiguration configuration) { }
    }
}
