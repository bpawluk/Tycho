using Microsoft.Extensions.DependencyInjection;
using System;
using Tycho;
using Tycho.Contract;
using Tycho.Structure;

namespace Test.Utils
{
    internal class TestModule : TychoModule
    {
        protected override void DeclareIncomingMessages(IInboxDefinition module, IServiceProvider services) { }
        protected override void DeclareOutgoingMessages(IOutboxDefinition module, IServiceProvider services) { }
        protected override void IncludeSubmodules(ISubstructureDefinition module, IServiceProvider services) { }
        protected override void RegisterServices(IServiceCollection services) { }
    }

    internal class OtherModule : TychoModule
    {
        protected override void DeclareIncomingMessages(IInboxDefinition module, IServiceProvider services) { }
        protected override void DeclareOutgoingMessages(IOutboxDefinition module, IServiceProvider services) { }
        protected override void IncludeSubmodules(ISubstructureDefinition module, IServiceProvider services) { }
        protected override void RegisterServices(IServiceCollection services) { }
    }

    internal class YetAnotherModule : TychoModule
    {
        protected override void DeclareIncomingMessages(IInboxDefinition module, IServiceProvider services) { }
        protected override void DeclareOutgoingMessages(IOutboxDefinition module, IServiceProvider services) { }
        protected override void IncludeSubmodules(ISubstructureDefinition module, IServiceProvider services) { }
        protected override void RegisterServices(IServiceCollection services) { }
    }
}
