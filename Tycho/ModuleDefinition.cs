using Microsoft.Extensions.DependencyInjection;
using System;
using Tycho.DependencyInjection;
using Tycho.Messaging;
using Tycho.Messaging.Contracts;
using Tycho.Structure;

namespace Tycho
{
    public abstract class ModuleDefinition
    {
        protected abstract void DeclareIncomingMessages(IInboxDefiner module, IServiceProvider services);

        protected abstract void DeclareOutgoingMessages(IOutboxDefiner module, IServiceProvider services);

        protected abstract void IncludeSubmodules(ISubmodulesDefiner submodules);

        protected abstract void RegisterServices(IServiceCollection services);

        public void Configure()
        {
            // TODO: Providing and using configuration data
        }

        public void Setup(Action<IOutboxConsumer> contractFullfilment)
        {
            // TODO: Move contract fullfilment logic here
        }

        public IModule Build()
        {
            var serviceCollection = new ServiceCollection();
            RegisterServices(serviceCollection);

            var submodulesBuilder = new SubmodulesBuilder();
            IncludeSubmodules(submodulesBuilder);

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var instanceCreator = new InstanceCreator(serviceProvider);

            var inboxRouter = new MessageRouter();
            var inboxBuilder = new InboxBuilder(instanceCreator, inboxRouter);
            DeclareIncomingMessages(inboxBuilder, serviceProvider);
            var externalBroker = inboxBuilder.Build();

            var outboxRouter = new MessageRouter();
            var outboxBuilder = new OutboxBuilder(outboxRouter);
            DeclareOutgoingMessages(outboxBuilder, serviceProvider);
            // contractFullfilment(outboxBuilder);
            var internalBroker = outboxBuilder.Build();

            var thisModuleType = typeof(Module<>).MakeGenericType(new Type[] { GetType() });
            return (Activator.CreateInstance(thisModuleType, internalBroker, externalBroker) as IModule)!;
        }
    }
}
