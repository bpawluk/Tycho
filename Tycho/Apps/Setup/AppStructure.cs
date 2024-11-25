using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Events.Routing;
using Tycho.Modules;
using Tycho.Requests.Broker;
using Tycho.Structure;
using Tycho.Structure.Data;

namespace Tycho.Apps.Setup
{
    internal class AppStructure : IAppStructure
    {
        private readonly Internals _internals;
        private readonly Globals _globals;

        private readonly Dictionary<Type, TychoModule> _submodules;

        public AppStructure(Internals internals, Globals globals)
        {
            _internals = internals;
            _globals = globals;
            _submodules = new Dictionary<Type, TychoModule>();
        }

        public IAppStructure Uses<TModule>()
            where TModule : TychoModule, new()
        {
            Use<TModule>(null, null);
            return this;
        }

        public IAppStructure Uses<TModule>(Action<IContractFulfillment> contractFulfillment)
            where TModule : TychoModule, new()
        {
            Use<TModule>(contractFulfillment, null);
            return this;
        }

        public IAppStructure Uses<TModule>(IModuleSettings settings)
            where TModule : TychoModule, new()
        {
            Use<TModule>(null, settings);
            return this;
        }

        public IAppStructure Uses<TModule>(
            Action<IContractFulfillment> contractFulfillment,
            IModuleSettings settings)
            where TModule : TychoModule, new()
        {
            Use<TModule>(contractFulfillment, settings);
            return this;
        }

        private void Use<TModule>(
            Action<IContractFulfillment>? contractFulfillment,
            IModuleSettings? settings)
            where TModule : TychoModule, new()
        {
            var submodule = new TModule().WithGlobals(_globals);

            if (settings != null)
            {
                submodule.WithSettings(settings);
            }

            var fulfiller = new ContractFulfillment<TModule>(_internals);
            contractFulfillment?.Invoke(fulfiller);

            var downStreamBroker = new DownStreamBroker<TModule>(_internals);
            submodule.FulfillContract(downStreamBroker);

            var parentEventRouter = new EventRouter(_internals);
            submodule.PassEventRouter(parentEventRouter);

            AddSubmodule(submodule);
        }

        public async Task Build()
        {
            var services = _internals.GetServiceCollection();
            await Task.WhenAll(_submodules.Values.Select(async module =>
            {
                var moduleInterface = typeof(IModule);
                var genericModuleInterface = typeof(IModule<>).MakeGenericType(module.GetType());
                var runningModule = await module.Run().ConfigureAwait(false);
                services.AddSingleton(moduleInterface, runningModule);
                services.AddSingleton(genericModuleInterface, runningModule);
            })).ConfigureAwait(false);
        }

        private void AddSubmodule(TychoModule submodule)
        {
            if (!_submodules.TryAdd(submodule.GetType(), submodule))
            {
                throw new InvalidOperationException(
                    $"{submodule.GetType().Name} is already defined " +
                    $"as a submodule for this module");
            }
        }
    }
}