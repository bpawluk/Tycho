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
        private Action<IOutboxConsumer>? _contractFullfilment;

        protected abstract void DeclareIncomingMessages(IInboxDefinition module, IServiceProvider services);

        protected abstract void DeclareOutgoingMessages(IOutboxDefinition module, IServiceProvider services);

        protected abstract void IncludeSubmodules(ISubstructureDefinition module, IServiceProvider services);

        protected abstract void RegisterServices(IServiceCollection services);

        protected virtual Task Startup(IServiceProvider services) { return Task.CompletedTask; }

        public TychoModule Configure()
        {
            // TODO: Providing and using configuration data
            return this;
        }

        public TychoModule Setup(Action<IOutboxConsumer> contractFullfilment)
        {
            _contractFullfilment = contractFullfilment;
            return this;
        }

        public async Task<IModule> Build()
        {
            var module = CreateModule();
            var services = CollectServices(module);
            module.SetSubmodules(await BuildModuleSubtree(services).ConfigureAwait(false));
            module.SetInternalBroker(BuildInternalMessageBroker(services));
            module.SetExternalBroker(BuildExternalMessageBroker(services));
            await Startup(services).ConfigureAwait(false);
            return module;
        }

        private IServiceProvider CollectServices(Module module)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IModule>(module.Internals);
            serviceCollection.AddSingleton(typeof(ISubmodule<>), typeof(SubmoduleProxy<>));
            RegisterServices(serviceCollection);
            return serviceCollection.BuildServiceProvider();
        }

        private async Task<IEnumerable<IModule>> BuildModuleSubtree(IServiceProvider serviceProvider)
        {
            var submodulesBuilder = new SubstructureBuilder();
            IncludeSubmodules(submodulesBuilder, serviceProvider);
            return await submodulesBuilder.Build().ConfigureAwait(false);
        }

        private IMessageBroker BuildInternalMessageBroker(IServiceProvider serviceProvider)
        {
            var outboxRouter = new MessageRouter();
            var outboxBuilder = new OutboxBuilder(outboxRouter);
            DeclareOutgoingMessages(outboxBuilder, serviceProvider);
            _contractFullfilment?.Invoke(outboxBuilder);
            return outboxBuilder.Build();
        }

        private IMessageBroker BuildExternalMessageBroker(IServiceProvider serviceProvider)
        {
            var inboxRouter = new MessageRouter();
            var instanceCreator = new InstanceCreator(serviceProvider);
            var inboxBuilder = new InboxBuilder(instanceCreator, inboxRouter);
            DeclareIncomingMessages(inboxBuilder, serviceProvider);
            return inboxBuilder.Build();
        }

        private Module CreateModule()
        {
            var thisModuleType = typeof(Submodule<>).MakeGenericType(new Type[] { GetType() });
            return (Activator.CreateInstance(thisModuleType) as Module)!;
        }
    }
}
