using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using TychoV2.Structure;

namespace TychoV2.Modules.Setup
{
    internal class ModuleBuilder
    {
        private readonly Type _moduleType;
        private readonly Internals _internals;

        private Action<IConfigurationBuilder>? _configurationDefinition;
        private Action<IContractFulfillment>? _contractFullfilment;
        private IServiceProvider? _contractFullfilmentServices;

        public IConfiguration? Configuration { get; private set; }

        public IServiceCollection Services => _internals.GetServiceCollection();

        public ModuleContract Contract { get; }

        public ModuleEvents Events { get; } = null!;

        public ModuleStructure Structure { get; } = null!;

        public ModuleBuilder(Type moduleDefinitionType)
        {
            _moduleType = typeof(Module<>).MakeGenericType(new Type[] { moduleDefinitionType });
            _internals = new Internals();
            Contract = new ModuleContract(_internals);
            Events = new ModuleEvents(_internals);
            Structure = new ModuleStructure(_internals);
        }

        public void WithConfiguration(Action<IConfigurationBuilder> configurationDefinition)
        {
            _configurationDefinition = configurationDefinition;
        }

        public void WithContractFulfillment(Action<IContractFulfillment> contractFulfillment, IServiceProvider contractFulfillmentServices)
        {
            _contractFullfilment = contractFulfillment;
            _contractFullfilmentServices = contractFulfillmentServices;
        }

        public void Init()
        {
            var configurationBuilder = new ConfigurationBuilder();
            _configurationDefinition?.Invoke(configurationBuilder);
            Configuration = configurationBuilder.Build();
        }

        public IModule Build()
        {
            var services = _internals.GetServiceCollection();
            var module = (IModule)Activator.CreateInstance(_moduleType, _internals);
            services.AddSingleton(module);

            Contract.Build();
            Events.Build();
            Structure.Build();
            _internals.Build();

            return module;
        }
    }
}
