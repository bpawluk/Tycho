using Microsoft.Extensions.Configuration;
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

namespace Tycho
{
    /// <summary>
    /// The base class for all module definitions in Tycho.NET
    /// </summary>
    public abstract class TychoModule
    {
        private readonly object _buildingLock = new object();
        private bool _wasAlreadyBuilt = false;

        private Action<IConfigurationBuilder>? _configurationDefinition;
        private Action<IOutboxConsumer>? _contractFullfilment;
        private IServiceProvider? _contractFullfilmentServices;

        /// <summary>
        /// Use this method to declare the interface that your module exposes to its clients
        /// </summary>
        /// <param name="module">An interface for declaring the incoming messages of your module</param>
        /// <param name="services">A provider of the services configured for your module</param>
        protected abstract void DeclareIncomingMessages(IInboxDefinition module, IServiceProvider services);

        /// <summary>
        /// Use this method to declare the contract that your module requires its clients to fulfill
        /// </summary>
        /// <param name="module">An interface for declaring the messages that your module sends out</param>
        /// <param name="services">A provider of the services configured for your module</param>
        protected abstract void DeclareOutgoingMessages(IOutboxDefinition module, IServiceProvider services);

        /// <summary>
        /// Use this method to configure the submodules that your module will be using
        /// </summary>
        /// <param name="module">An interface for defining the substructure of your module</param>
        /// <param name="services">A provider of the services configured for your module</param>
        protected abstract void IncludeSubmodules(ISubstructureDefinition module, IServiceProvider services);

        /// <summary>
        /// Use this method to configure the services required by your module's code
        /// </summary>
        /// <param name="services">An interface for defining the collection of services available for your module</param>
        /// <param name="configuration">The configuration defined for your module</param>
        protected abstract void RegisterServices(IServiceCollection services, IConfiguration configuration);

        /// <summary>
        /// Override this method if you need to run some code just before your module is created
        /// </summary>
        /// <param name="services">A provider of the services configured for your module</param>
        protected virtual Task Startup(IServiceProvider services) { return Task.CompletedTask; }

        /// <summary>
        /// Call this method before building your module to define its configuration
        /// </summary>
        /// <param name="configurationDefinition">A method that defines your module's configuration</param>
        public TychoModule Configure(Action<IConfigurationBuilder> configurationDefinition)
        {
            _configurationDefinition = configurationDefinition;
            return this;
        }

        /// <summary>
        /// Call this method before building your module to handle the messages required by its contract
        /// </summary>
        /// <param name="contractFulfillment">A method that conducts the contract fulfillment</param>
        /// <param name="serviceProvider">A service provider to be used for message handler instantiation</param>
        public TychoModule FulfillContract(Action<IOutboxConsumer> contractFulfillment, IServiceProvider? serviceProvider = null)
        {
            _contractFullfilment = contractFulfillment;
            _contractFullfilmentServices = serviceProvider;
            return this;
        }

        /// <summary>
        /// Builds your module accordingly to its definition
        /// </summary>
        /// <returns>A fresh and ready-to-use instance of your module</returns>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        public async Task<IModule> Build()
        {
            EnsureItIsBuiltOnlyOnce();

            var module = CreateModule();
            var configuration = BuildConfiguration();
            var internalServices = CollectInternalServices(module, configuration);

            var submodules = await BuildSubstructure(internalServices).ConfigureAwait(false);
            module.SetSubmodules(submodules);

            module.SetInternalBroker(BuildMessageOutbox(internalServices));
            module.SetExternalBroker(BuildMessageInbox(internalServices));

            await Startup(internalServices).ConfigureAwait(false);
            return module;
        }

        private IConfiguration BuildConfiguration()
        {
            var configurationBuilder = new ConfigurationBuilder();
            _configurationDefinition?.Invoke(configurationBuilder);
            return configurationBuilder.Build();
        }

        private IServiceProvider CollectInternalServices(Module module, IConfiguration configuration)
        {
            var services = new ServiceCollection();
            services.AddSingleton(configuration);
            services.AddModuleInternals(module);
            services.AddSubmodules();
            RegisterServices(services, configuration);
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
            var instanceCreator = new InstanceCreator(_contractFullfilmentServices);
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
            var thisModuleType = typeof(Module<>).MakeGenericType(new Type[] { GetType() });
            return (Activator.CreateInstance(thisModuleType) as Module)!;
        }

        private void EnsureItIsBuiltOnlyOnce()
        {
            lock (_buildingLock)
            {
                if (_wasAlreadyBuilt) throw new InvalidOperationException("This module has already been built");
                _wasAlreadyBuilt = true;
            }
        }
    }
}
