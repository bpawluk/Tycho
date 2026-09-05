using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tycho.Events.Broker;
using Tycho.Hosting;
using Tycho.Hosting.Services;
using Tycho.Modules.Instance;
using Tycho.Requests.Broker;
using Tycho.Structure;
using Tycho.Structure.Parent;

namespace Tycho.Modules.Setup
{
    internal sealed class ModuleBuilder
    {
        private readonly Type _moduleDefinitionType;
        private readonly Type _moduleType;
        private readonly HostLifecycleCallbacks _lifecycleCallbacks;

        private Func<HostApplicationBuilder>? _createHostBuilderDelegate;
        private Action<IServiceProvider?, HostApplicationBuilder>? _configureHostDelegate;
        private Action<IModuleContract>? _configureContractDelegate;
        private Action<IModuleEvents>? _configureEventsDelegate;
        private Action<IModuleStructure>? _configureStructureDelegate;
        private Action<IServiceCollection>? _registerServicesDelegate;
        private IRequestBroker? _contractFulfillingBroker;
        private IEventBroker? _parentEventBroker;
        private int _built;

        public ModuleBuilder(Type moduleDefinitionType)
        {
            _moduleDefinitionType = moduleDefinitionType;
            _moduleType = typeof(Module<>).MakeGenericType(moduleDefinitionType);
            _lifecycleCallbacks = new HostLifecycleCallbacks();
        }

        public ModuleBuilder WithHostBuilder(Func<HostApplicationBuilder> createHostBuilder)
        {
            _createHostBuilderDelegate = createHostBuilder ?? throw new ArgumentNullException(nameof(createHostBuilder));
            return this;
        }

        public ModuleBuilder WithHostConfiguration(Action<IServiceProvider?, HostApplicationBuilder> configureHost)
        {
            _configureHostDelegate = configureHost ?? throw new ArgumentNullException(nameof(configureHost));
            return this;
        }

        public ModuleBuilder WithContract(Action<IModuleContract> configureContract, IRequestBroker? contractFulfillingBroker)
        {
            _configureContractDelegate = configureContract ?? throw new ArgumentNullException(nameof(configureContract));
            _contractFulfillingBroker = contractFulfillingBroker;
            return this;
        }

        public ModuleBuilder WithEvents(Action<IModuleEvents> configureEvents, IEventBroker? parentEventBroker)
        {
            _configureEventsDelegate = configureEvents ?? throw new ArgumentNullException(nameof(configureEvents));
            _parentEventBroker = parentEventBroker;
            return this;
        }

        public ModuleBuilder WithStructure(Action<IModuleStructure> configureStructure)
        {
            _configureStructureDelegate = configureStructure ?? throw new ArgumentNullException(nameof(configureStructure));
            return this;
        }

        public ModuleBuilder WithServices(Action<IServiceCollection> registerServices)
        {
            _registerServicesDelegate = registerServices ?? throw new ArgumentNullException(nameof(registerServices));
            return this;
        }

        public ModuleBuilder WithStartup(Func<IServiceProvider, CancellationToken, Task> startup)
        {
            _lifecycleCallbacks.WithStartup(startup);
            return this;
        }

        public ModuleBuilder WithCleanup(Func<IServiceProvider, CancellationToken, Task> cleanup)
        {
            _lifecycleCallbacks.WithCleanup(cleanup);
            return this;
        }

        public IModule Build(IServiceProvider? parentServiceProvider = null)
        {
            if (Interlocked.Exchange(ref _built, 1) != 0)
            {
                throw new InvalidOperationException("The module has already been built.");
            }

            HostApplicationBuilder hostBuilder = _createHostBuilderDelegate?.Invoke() ?? throw new InvalidOperationException("The module host builder has not been configured.");
            var internals = new Internals(_moduleDefinitionType, hostBuilder);

            hostBuilder.Services.AddSingleton(internals);
            hostBuilder.Services.AddSingleton<IHostLifecycleCallbacks>(_lifecycleCallbacks);
            hostBuilder.Services.AddHostedService<HostLifecycleCallbacksService>();

            if (_contractFulfillingBroker == null || _parentEventBroker == null)
            {
                throw new InvalidOperationException("The module parent has not been configured.");
            }

            var structure = new ModuleStructure(internals);
            _configureStructureDelegate?.Invoke(structure);
            structure.Build();

            var contract = new ModuleContract(internals);
            contract.WithContractFulfillment(_contractFulfillingBroker);
            _configureContractDelegate?.Invoke(contract);

            var events = new ModuleEvents(internals);
            events.WithParentEventBroker(_parentEventBroker);
            _configureEventsDelegate?.Invoke(events);
            hostBuilder.Services.AddSingleton<IParentReference>(new ParentReference(events.ParentEventBroker, contract.ContractFulfillingBroker));
            events.Build();

            _registerServicesDelegate?.Invoke(hostBuilder.Services);
            _configureHostDelegate?.Invoke(parentServiceProvider, hostBuilder);
            internals.Build();

            return Activator.CreateInstance(_moduleType, internals) as IModule ?? throw new InvalidOperationException($"Failed to create an instance of {_moduleType.Name}.");
        }
    }
}
