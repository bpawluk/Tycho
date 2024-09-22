using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NewTycho.Modules.Contract;
using NewTycho.Structure;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NewTycho.Modules
{
    public abstract class TychoModule
    {
        private readonly object _buildingLock = new object();
        private bool _wasAlreadyBuilt = false;

        private Action<IConfigurationBuilder>? _configurationDefinition;
        private Action<IOutboxConsumer>? _contractFullfilment;
        private IServiceProvider? _contractFullfilmentServices;

        private IConfiguration? _configuration;
        protected IConfiguration Configuration => _configuration ?? throw new InvalidOperationException("Module configuration not yet available");

        /// <summary>
        /// TODO
        /// </summary>
        protected abstract void DefineInbox(IModuleInbox outbox);

        /// <summary>
        /// TODO
        /// </summary>
        protected abstract void DefineOutbox(IModuleOutbox inbox);

        /// <summary>
        /// Use this method to configure the modules that your module will be using
        /// </summary>
        /// <param name="structure">An interface for defining the structure of your module</param>
        protected abstract void IncludeModules(IModuleStructure structure);

        /// <summary>
        /// Use this method to configure the services required by your module
        /// </summary>
        /// <param name="collection">An interface for defining the collection of services available for your module</param>
        protected abstract void RegisterServices(IServiceCollection collection);

        /// <summary>
        /// Override this method if you need to run some code just before your module is created
        /// </summary>
        protected virtual Task Startup(IServiceProvider services) { return Task.CompletedTask; }

        internal async Task<Module> Build()
        {
            EnsureItIsBuiltOnlyOnce();
            BuildConfiguration();

            var module = new Module();
            BuildServiceCollection(module);
            BuildMessageOutbox(module);
            BuildMessageInbox(module);
            await BuildSubstructure(module).ConfigureAwait(false);
            module.Start();

            await Startup(module.Services).ConfigureAwait(false);
            return module;
        }

        internal TychoModule Configure(Action<IConfigurationBuilder> configurationDefinition)
        {
            _configurationDefinition = configurationDefinition;
            return this;
        }

        internal TychoModule FulfillContract(Action<IOutboxConsumer> contractFulfillment, IServiceProvider serviceProvider)
        {
            _contractFullfilment = contractFulfillment;
            _contractFullfilmentServices = serviceProvider;
            return this;
        }

        private void BuildConfiguration()
        {
            var configurationBuilder = new ConfigurationBuilder();
            _configurationDefinition?.Invoke(configurationBuilder);
            _configuration = configurationBuilder.Build();
        }

        private void BuildServiceCollection(Module module)
        {
            var services = new ServiceCollection();
            services.AddSingleton(_configuration!);
            RegisterServices(services);
            module.SetServices(services);
        }

        private void BuildMessageOutbox(Module module)
        {
            var outboxBuilder = new ModuleOutboxBuilder(_contractFullfilmentServices);

            DefineOutbox(outboxBuilder);
            _contractFullfilment?.Invoke(outboxBuilder);

            // eventBroker

            var requestBroker = outboxBuilder.RequestOutboxBuilder.Build();
            module.SetExternalRequestBroker(requestBroker);
        }

        private void BuildMessageInbox(IServiceProvider internalServices)
        {
            var inboxBuilder = new InboxBuilder(instanceCreator, inboxRouter);
            HandleIncomingMessages(inboxBuilder, internalServices);
            return inboxBuilder.Build();
        }

        private async Task<IEnumerable<IModule>> BuildSubstructure()
        {
            var builder = new StructureBuilder(Services);
            IncludeModules(builder);
            return await builder.Build().ConfigureAwait(false);
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
                if (_wasAlreadyBuilt) throw new InvalidOperationException("This module is already built");
                _wasAlreadyBuilt = true;
            }
        }
    }
}
