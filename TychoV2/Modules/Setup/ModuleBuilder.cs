using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using TychoV2.Events.Routing;
using TychoV2.Modules.Instance;
using TychoV2.Requests.Broker;
using TychoV2.Structure;

namespace TychoV2.Modules.Setup
{
    internal class ModuleBuilder
    {
        private readonly Type _moduleType;
        private readonly Internals _internals;

        private Action<IConfigurationBuilder>? _configurationDefinition;

        public IConfiguration? Configuration { get; private set; }

        public IServiceCollection Services => _internals.GetServiceCollection();

        public ModuleContract Contract { get; }

        public ModuleEvents Events { get; } = null!;

        public ModuleStructure Structure { get; } = null!;

        public ModuleBuilder(Type moduleDefinitionType)
        {
            _moduleType = typeof(Module<>).MakeGenericType(new Type[] { moduleDefinitionType });
            _internals = new Internals(moduleDefinitionType.FullName);
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
            var services = _internals.GetServiceCollection();
            var configurationBuilder = new ConfigurationBuilder();
            _configurationDefinition?.Invoke(configurationBuilder);
            Configuration = configurationBuilder.Build();
            services.AddSingleton(Configuration);
        }

        public async Task<IModule> Build()
        {
            var services = _internals.GetServiceCollection();
            var module = (IModule)Activator.CreateInstance(_moduleType, _internals);

            await Structure.Build();

            var parentProxy = new ParentProxy(Contract.ContractFulfillingBroker, Events.ParentEventRouter);
            services.AddSingleton<IParent>(parentProxy);
            _internals.Build();

            return module;
        }
    }
}
