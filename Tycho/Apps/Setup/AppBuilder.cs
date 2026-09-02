using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tycho.Apps.Instance;
using Tycho.Hosting;
using Tycho.Hosting.Services;
using Tycho.Structure;

namespace Tycho.Apps.Setup
{
    internal class AppBuilderBase : IAppBuilderBase
    {
        private readonly Type _appDefinitionType;
        private readonly Type _appType;
        private readonly HostLifecycleCallbacks _lifecycleCallbacks;

        private Func<HostApplicationBuilder>? _createHostBuilderDelegate;
        private Action<IServiceProvider?, HostApplicationBuilder>? _configureHostDelegate;
        private Action<IAppContract>? _configureContractDelegate;
        private Action<IAppEvents>? _configureEventsDelegate;
        private Action<IAppStructure>? _configureStructureDelegate;
        private Action<IServiceCollection>? _registerServicesDelegate;
        private int _built;

        public AppBuilderBase(Type appDefinitionType)
        {
            _appDefinitionType = appDefinitionType;
            _appType = typeof(App<>).MakeGenericType(appDefinitionType);
            _lifecycleCallbacks = new HostLifecycleCallbacks();
        }

        public AppBuilderBase WithHostBuilder(Func<HostApplicationBuilder> createHostBuilder)
        {
            _createHostBuilderDelegate = createHostBuilder ?? throw new ArgumentNullException(nameof(createHostBuilder));
            return this;
        }

        public AppBuilderBase WithHostConfiguration(Action<IServiceProvider?, HostApplicationBuilder> configureHost)
        {
            _configureHostDelegate = configureHost ?? throw new ArgumentNullException(nameof(configureHost));
            return this;
        }

        public AppBuilderBase WithContract(Action<IAppContract> configureContract)
        {
            _configureContractDelegate = configureContract ?? throw new ArgumentNullException(nameof(configureContract));
            return this;
        }

        public AppBuilderBase WithEvents(Action<IAppEvents> configureEvents)
        {
            _configureEventsDelegate = configureEvents ?? throw new ArgumentNullException(nameof(configureEvents));
            return this;
        }

        public AppBuilderBase WithStructure(Action<IAppStructure> configureStructure)
        {
            _configureStructureDelegate = configureStructure ?? throw new ArgumentNullException(nameof(configureStructure));
            return this;
        }

        public AppBuilderBase WithServices(Action<IServiceCollection> registerServices)
        {
            _registerServicesDelegate = registerServices ?? throw new ArgumentNullException(nameof(registerServices));
            return this;
        }

        public AppBuilderBase WithStartup(Func<IServiceProvider, CancellationToken, Task> startup)
        {
            _lifecycleCallbacks.WithStartup(startup);
            return this;
        }

        public AppBuilderBase WithCleanup(Func<IServiceProvider, CancellationToken, Task> cleanup)
        {
            _lifecycleCallbacks.WithCleanup(cleanup);
            return this;
        }

        public IApp Build(IServiceProvider? parentServiceProvider)
        {
            if (Interlocked.Exchange(ref _built, 1) != 0)
            {
                throw new InvalidOperationException("The app has already been built.");
            }

            HostApplicationBuilder hostBuilder = _createHostBuilderDelegate?.Invoke() ?? throw new InvalidOperationException("The app host builder has not been configured.");
            var internals = new Internals(_appDefinitionType, hostBuilder);

            hostBuilder.Services.AddSingleton(internals);
            hostBuilder.Services.AddSingleton<IHostLifecycleCallbacks>(_lifecycleCallbacks);
            hostBuilder.Services.AddHostedService<HostLifecycleCallbacksService>();

            var structure = new AppStructure(internals);
            _configureStructureDelegate?.Invoke(structure);
            structure.Build();

            var contract = new AppContract(internals);
            _configureContractDelegate?.Invoke(contract);

            var events = new AppEvents(internals);
            _configureEventsDelegate?.Invoke(events);
            events.Build();

            _registerServicesDelegate?.Invoke(hostBuilder.Services);
            _configureHostDelegate?.Invoke(parentServiceProvider, hostBuilder);
            internals.Build();

            return Activator.CreateInstance(_appType, internals) as IApp ?? throw new InvalidOperationException($"Failed to create an instance of {_appType.Name}.");
        }
    }
}
