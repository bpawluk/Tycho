using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tycho.Events.Routing;
using Tycho.Hosting.Services;
using Tycho.Modules.Instance;
using Tycho.Requests.Broker;
using Tycho.Structure;

namespace Tycho.Modules.Setup
{
    internal sealed class ModuleStructure : IModuleStructure
    {
        private readonly Internals _internals;
        private readonly List<TychoModule> _submodules = new();
        private readonly HashSet<Type> _submoduleTypes = new();

        public ModuleStructure(Internals internals)
        {
            _internals = internals;
        }

        public IModuleStructure Uses<TModule>(string? instanceSuffix = null) where TModule : TychoModule, new()
        {
            Use<TModule>(null, null, instanceSuffix);
            return this;
        }

        public IModuleStructure Uses<TModule>(Action<IContractFulfillment> contractFulfillment, string? instanceSuffix = null)
            where TModule : TychoModule, new()
        {
            Use<TModule>(contractFulfillment, null, instanceSuffix);
            return this;
        }

        public IModuleStructure Uses<TModule>(IModuleSettings settings, string? instanceSuffix = null)
            where TModule : TychoModule, new()
        {
            Use<TModule>(null, settings, instanceSuffix);
            return this;
        }

        public IModuleStructure Uses<TModule>(
            Action<IContractFulfillment> contractFulfillment,
            IModuleSettings settings,
            string? instanceSuffix = null)
            where TModule : TychoModule, new()
        {
            Use<TModule>(contractFulfillment, settings, instanceSuffix);
            return this;
        }

        public void Build()
        {
            IServiceCollection services = _internals.GetHostBuilder().Services;

            foreach (TychoModule moduleDefinition in _submodules)
            {
                ModuleBuilder moduleBuilder = moduleDefinition.CreateModuleBuilder().WithControlPlane(_internals.ControlPlane);
                Type genericModuleInterface = typeof(IModule<>).MakeGenericType(moduleDefinition.GetType());

                services.AddSingleton(genericModuleInterface, provider => moduleBuilder.Build(provider));
                services.AddSingleton(typeof(IModule), provider => provider.GetRequiredService(genericModuleInterface));

                Type lifecycleService = typeof(ModuleHostedLifecycleService<>).MakeGenericType(moduleDefinition.GetType());
                services.AddSingleton(typeof(IHostedService), lifecycleService);
            }
        }

        private void Use<TModule>(
            Action<IContractFulfillment>? contractFulfillment,
            IModuleSettings? settings,
            string? instanceSuffix)
            where TModule : TychoModule, new()
        {
            if (instanceSuffix != null && string.IsNullOrWhiteSpace(instanceSuffix))
            {
                throw new ArgumentException("Module instance suffix cannot be empty.", nameof(instanceSuffix));
            }

            var submodule = new TModule();
            if (settings != null)
            {
                submodule.WithSettings(settings);
            }

            var fulfiller = new ContractFulfillment<TModule>(_internals);
            contractFulfillment?.Invoke(fulfiller);

            submodule.FulfillContract(new DownStreamBroker<TModule>(_internals));
            submodule.PassEventRouter(new EventRouter(_internals));
            submodule.WithInstanceSuffix(instanceSuffix);

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
