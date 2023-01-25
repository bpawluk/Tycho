using Microsoft.Extensions.DependencyInjection;
using System;
using Tycho;
using Tycho.Contract;
using Tycho.Structure;

namespace Test.Utils
{
    public class TestModule : TychoModule
    {
        protected override void DeclareIncomingMessages(IInboxDefinition module, IServiceProvider services) { }
        protected override void DeclareOutgoingMessages(IOutboxDefinition module, IServiceProvider services) { }
        protected override void IncludeSubmodules(ISubstructureDefinition module, IServiceProvider services) { }
        protected override void RegisterServices(IServiceCollection services) { }
    }

    public class OtherModule : TychoModule
    {
        protected override void DeclareIncomingMessages(IInboxDefinition module, IServiceProvider services) { }
        protected override void DeclareOutgoingMessages(IOutboxDefinition module, IServiceProvider services) { }
        protected override void IncludeSubmodules(ISubstructureDefinition module, IServiceProvider services) { }
        protected override void RegisterServices(IServiceCollection services) { }
    }

    public class YetAnotherModule : TychoModule
    {
        protected override void DeclareIncomingMessages(IInboxDefinition module, IServiceProvider services) { }
        protected override void DeclareOutgoingMessages(IOutboxDefinition module, IServiceProvider services) { }
        protected override void IncludeSubmodules(ISubstructureDefinition module, IServiceProvider services) { }
        protected override void RegisterServices(IServiceCollection services) { }
    }
}
