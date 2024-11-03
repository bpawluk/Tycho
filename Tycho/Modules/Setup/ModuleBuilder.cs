using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Events.Routing;
using Tycho.Modules.Instance;
using Tycho.Requests.Broker;
using Tycho.Structure;

namespace Tycho.Modules.Setup
{
    internal class ModuleBuilder
    {
        private readonly Internals _internals;
        private readonly Type _moduleType;

        private Action<IConfigurationBuilder>? _configurationDefinition;

        public IConfiguration? Configuration { get; private set; }

        public IServiceCollection Services => _internals.GetServiceCollection();

        public ModuleContract Contract { get; }

        public ModuleEvents Events { get; }

        public ModuleStructure Structure { get; }

        public ModuleBuilder(Type moduleDefinitionType)
        {
            _moduleType = typeof(Module<>).MakeGenericType(moduleDefinitionType);
            _internals = new Internals(moduleDefinitionType);
            Contract = new ModuleContract(_internals);
            Events = new ModuleEvents(_internals);
            Structure = new ModuleStructure(_internals);
        }

        public void WithConfiguration(Action<IConfigurationBuilder> configurationDefinition)
        {
            _configurationDefinition = configurationDefinition;
        }

        public void WithContractFulfillment(IRequestBroker contractFulfillingBroker)
        {
            Contract.WithContractFulfillment(contractFulfillingBroker);
        }

        public void WithParentEventRouter(IEventRouter parentEventRouter)
        {
            Events.WithParentEventRouter(parentEventRouter);
        }

        public void Init()
        {
            var configurationBuilder = new ConfigurationBuilder();
            _configurationDefinition?.Invoke(configurationBuilder);
            Configuration = configurationBuilder.Build();

            var parentProxy = new ParentProxy(Contract.ContractFulfillingBroker, Events.ParentEventRouter);

            _internals.GetServiceCollection()
                .AddSingleton(Configuration)
                .AddSingleton<IParent>(parentProxy)
                .AddSingleton(_internals);
        }

        public async Task<IModule> Build()
        {
            var module = (IModule)Activator.CreateInstance(_moduleType, _internals);

            await Contract.Build().ConfigureAwait(false);
            await Events.Build().ConfigureAwait(false);
            await Structure.Build().ConfigureAwait(false);
            _internals.Build();

            return module;
        }
    }
}