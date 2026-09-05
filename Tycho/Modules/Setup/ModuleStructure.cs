using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tycho.Events.Broker;
using Tycho.Hosting.Services;
using Tycho.Identity.Modules;
using Tycho.Modules.Instance;
using Tycho.Requests.Broker;
using Tycho.Structure;

namespace Tycho.Modules.Setup
{
    internal sealed class ModuleStructure : IModuleStructure
    {
        private readonly Internals _internals;
        private readonly List<TychoModule> _submodules = new List<TychoModule>();
        private readonly HashSet<Type> _submoduleTypes = new HashSet<Type>();

        public ModuleStructure(Internals internals)
        {
            _internals = internals;
        }

        public IModuleStructure Uses<TModule>() where TModule : TychoModule, new()
        {
            Use<TModule>(null, null);
            return this;
        }

        public IModuleStructure Uses<TModule>(Action<IContractFulfillment> contractFulfillment)
            where TModule : TychoModule, new()
        {
            Use<TModule>(contractFulfillment, null);
            return this;
        }

        public IModuleStructure Uses<TModule>(IModuleSettings settings)
            where TModule : TychoModule, new()
        {
            Use<TModule>(null, settings);
            return this;
        }

        public IModuleStructure Uses<TModule>(
            Action<IContractFulfillment> contractFulfillment,
            IModuleSettings settings)
            where TModule : TychoModule, new()
        {
            Use<TModule>(contractFulfillment, settings);
            return this;
        }

        public void Build()
        {
            IServiceCollection services = _internals.GetHostBuilder().Services;
            services.AddTransient<IModuleProvider, ModuleProvider>();

            foreach (TychoModule moduleDefinition in _submodules)
            {
                ModuleBuilder moduleBuilder = moduleDefinition.CreateModuleBuilder();
                Type genericModuleInterface = typeof(IModule<>).MakeGenericType(moduleDefinition.GetType());

                services.AddSingleton(genericModuleInterface, provider => moduleBuilder.Build(provider));
                services.AddSingleton(typeof(IModule), provider => provider.GetRequiredService(genericModuleInterface));

                Type lifecycleService = typeof(ModuleHostedLifecycleService<>).MakeGenericType(moduleDefinition.GetType());
                services.AddSingleton(typeof(IHostedService), lifecycleService);
            }
        }

        private void Use<TModule>(
            Action<IContractFulfillment>? contractFulfillment,
            IModuleSettings? settings)
            where TModule : TychoModule, new()
        {
            var submodule = new TModule();
            if (settings != null)
            {
                submodule.WithSettings(settings);
            }

            var fulfiller = new ContractFulfillment<TModule>(_internals);
            contractFulfillment?.Invoke(fulfiller);

            submodule.FulfillContract(new DownStreamBroker<TModule>(_internals));
            submodule.PassEventBroker(new EventBroker(_internals));

            AddSubmodule(submodule);
        }

        private void AddSubmodule(TychoModule submodule)
        {
            Type submoduleType = submodule.GetType();
            if (!_submoduleTypes.Add(submoduleType))
            {
                throw new InvalidOperationException($"{submoduleType.Name} is already defined as a submodule for this module");
            }
            _submodules.Add(submodule);
        }
    }
}
