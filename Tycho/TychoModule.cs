using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tycho.Contract;
using Tycho.Contract.Builders;
using Tycho.DependencyInjection;
using Tycho.Messaging;
using Tycho.Structure;
using Tycho.Structure.Builders;
using Tycho.Structure.Modules;
using Tycho.Structure.Submodules;

namespace Tycho
{
    public abstract class TychoModule
    {
        private IServiceProvider? _externalServices;
        private Action<IOutboxConsumer>? _contractFullfilment;

        protected abstract void DeclareIncomingMessages(IInboxDefinition module, IServiceProvider services);

        protected abstract void DeclareOutgoingMessages(IOutboxDefinition module, IServiceProvider services);

        protected abstract void IncludeSubmodules(ISubstructureDefinition module, IServiceProvider services);

        protected abstract void RegisterServices(IServiceCollection services);

        protected virtual Task Startup(IServiceProvider services) { return Task.CompletedTask; }

        public TychoModule UseServices(IServiceProvider services)
        {
            _externalServices = services;
            return this;
        }

        public TychoModule FulfillContract(Action<IOutboxConsumer> contractFullfilment)
        {
            _contractFullfilment = contractFullfilment;
            return this;
        }

        public async Task<IModule> Build()
        {
            var module = CreateModule();
            var internalServices = CollectInternalServices(module);

            var submodules = await BuildSubstructure(internalServices).ConfigureAwait(false);
            module.SetSubmodules(submodules);

            module.SetInternalBroker(BuildMessageOutbox(internalServices));
            module.SetExternalBroker(BuildMessageInbox(internalServices));

            await Startup(internalServices).ConfigureAwait(false);
            return module;
        }

        private IServiceProvider CollectInternalServices(Module module)
        {
            var services = new ServiceCollection();
            services.AddModuleInternals(module);
            services.AddSubmodules();
            RegisterServices(services);
            return services.BuildServiceProvider();
        }

        private async Task<IEnumerable<IModule>> BuildSubstructure(IServiceProvider internalServices)
        {
            var builder = new SubstructureBuilder(internalServices);
            IncludeSubmodules(builder, internalServices);
            return await builder.Build().ConfigureAwait(false);
        }

        private IMessageBroker BuildMessageOutbox(IServiceProvider internalServices)
        {
            var outboxRouter = new MessageRouter();
            var instanceCreator = new InstanceCreator(_externalServices);
            var outboxBuilder = new OutboxBuilder(instanceCreator, outboxRouter);
            DeclareOutgoingMessages(outboxBuilder, internalServices);
            _contractFullfilment?.Invoke(outboxBuilder);
            return outboxBuilder.Build();
        }

        private IMessageBroker BuildMessageInbox(IServiceProvider internalServices)
        {
            var inboxRouter = new MessageRouter();
            var instanceCreator = new InstanceCreator(internalServices);
            var inboxBuilder = new InboxBuilder(instanceCreator, inboxRouter);
            DeclareIncomingMessages(inboxBuilder, internalServices);
            return inboxBuilder.Build();
        }

        private Module CreateModule()
        {
            var thisModuleType = typeof(Submodule<>).MakeGenericType(new Type[] { GetType() });
            return (Activator.CreateInstance(thisModuleType) as Module)!;
        }
    }
}
