using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TychoV2.Modules;
using TychoV2.Requests.Broker;
using TychoV2.Structure;

namespace TychoV2.Apps.Setup
{
    internal class AppStructure : IAppStructure
    {
        private readonly Internals _internals;
        private readonly Dictionary<Type, TychoModule> _submodules;

        public AppStructure(Internals internals)
        {
            _internals = internals;
            _submodules = new Dictionary<Type, TychoModule>();
        }

        public IAppStructure AddModule<TModule>(
            Action<IContractFulfillment>? contractFulfillment = null,
            Action<IConfigurationBuilder>? configurationDefinition = null)
            where TModule : TychoModule, new()
        {
            var submodule = new TModule();

            var fulfiller = new ContractFulfillment<TModule>(_internals);
            contractFulfillment?.Invoke(fulfiller);

            var downStreamBroker = new DownStreamBroker<TModule>(_internals);
            submodule.FulfillContract(downStreamBroker);

            if (configurationDefinition != null)
            {
                submodule.Configure(configurationDefinition);
            }

            AddSubmodule(submodule);

            return this;
        }

        public async Task Build()
        {
            var services = _internals.GetServiceCollection();
            await Task.WhenAll(_submodules.Values.Select(async module =>
            {
                var runningModule = await module.Run();
                var interfaceType = typeof(IModule<>).MakeGenericType(module.GetType());
                services.AddSingleton(interfaceType, runningModule);

            })).ConfigureAwait(false);
        }

        private void AddSubmodule(TychoModule submodule)
        {
            if (!_submodules.TryAdd(submodule.GetType(), submodule))
            {
                throw new InvalidOperationException(submodule.GetType().Name +
                    " is already defined as a submodule for this module");
            }
        }
    }
}
