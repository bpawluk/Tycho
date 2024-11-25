using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Events.Routing;
using Tycho.Modules.Instance;
using Tycho.Requests.Broker;
using Tycho.Structure;
using Tycho.Structure.Data;

namespace Tycho.Modules.Setup
{
    internal class ModuleBuilder
    {
        private readonly Type _moduleType;
        private readonly Internals _internals;

        private Func<IServiceProvider, Task>? _cleanup;

        public Globals Globals { get; private set; }

        public IModuleSettings? Settings { get; private set; }

        public ModuleContract Contract { get; }

        public ModuleEvents Events { get; }

        public ModuleStructure Structure { get; }

        public IServiceCollection Services => _internals.GetServiceCollection();

        public ModuleBuilder(Type moduleDefinitionType)
        {
            _moduleType = typeof(Module<>).MakeGenericType(moduleDefinitionType);
            _internals = new Internals(moduleDefinitionType);
            Globals = new Globals();
            Settings = null!;
            Contract = new ModuleContract(_internals);
            Events = new ModuleEvents(_internals);
            Structure = new ModuleStructure(_internals, Globals);
        }

        public ModuleBuilder WithGlobals(Globals globals)
        {
            Globals.Configuration = globals.Configuration;
            Globals.LoggingSetup = globals.LoggingSetup;
            return this;
        }

        public ModuleBuilder WithSettings(IModuleSettings settings)
        {
            Settings = settings;
            return this;
        }

        public ModuleBuilder WithContractFulfillment(IRequestBroker contractFulfillingBroker)
        {
            Contract.WithContractFulfillment(contractFulfillingBroker);
            return this;
        }

        public ModuleBuilder WithParentEventRouter(IEventRouter parentEventRouter)
        {
            Events.WithParentEventRouter(parentEventRouter);
            return this;
        }

        public ModuleBuilder WithCleanup(Func<IServiceProvider, Task> cleanup)
        {
            _cleanup = cleanup;
            return this;
        }

        public ModuleBuilder Init()
        {
            var parentProxy = new ParentProxy(Contract.ContractFulfillingBroker, Events.ParentEventRouter);
            var services = _internals.GetServiceCollection();

            if (Globals.LoggingSetup != null)
            {
                services.AddLogging(Globals.LoggingSetup);
            }

            services.AddSingleton<IParent>(parentProxy)
                    .AddSingleton(_internals);

            return this;
        }

        public async Task<IModule> Build()
        {
            var module = (IModule)Activator.CreateInstance(_moduleType, _internals, _cleanup);

            await Contract.Build().ConfigureAwait(false);
            await Events.Build().ConfigureAwait(false);
            await Structure.Build().ConfigureAwait(false);
            _internals.Build();

            return module;
        }
    }
}