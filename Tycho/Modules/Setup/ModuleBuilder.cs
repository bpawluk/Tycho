using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Events.Broker;
using Tycho.Modules.Instance;
using Tycho.Requests.Broker;
using Tycho.Structure;
using Tycho.Structure.Parent;

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

        public ModuleBuilder WithParentEventBroker(IEventBroker parentEventBroker)
        {
            Events.WithParentEventBroker(parentEventBroker);
            return this;
        }

        public ModuleBuilder WithCleanup(Func<IServiceProvider, Task> cleanup)
        {
            _cleanup = cleanup;
            return this;
        }

        public ModuleBuilder Init()
        {
            var parent = new ParentReference(Events.ParentEventBroker, Contract.ContractFulfillingBroker);

            if (Globals.LoggingSetup != null)
            {
                Services.AddLogging(Globals.LoggingSetup);
            }

            Services.AddSingleton<IParentReference>(parent)
                    .AddSingleton(_internals);

            return this;
        }

        public async Task<IModule> BuildAsync()
        {
            IModule module = Activator.CreateInstance(_moduleType, _internals, _cleanup) as IModule ?? throw new InvalidOperationException($"Failed to create an instance of {_moduleType.Name}.");
            await Contract.BuildAsync().ConfigureAwait(false);
            await Events.BuildAsync().ConfigureAwait(false);
            await Structure.BuildAsync().ConfigureAwait(false);
            _internals.Build();

            return module;
        }
    }
}
