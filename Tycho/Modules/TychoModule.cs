using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tycho.Events.Broker;
using Tycho.Hosting;
using Tycho.Hosting.Files;
using Tycho.Modules.Setup;
using Tycho.Requests.Broker;
using Tycho.Utils;

namespace Tycho.Modules
{
    /// <summary>
    /// Base class for defining a Tycho module.
    /// </summary>
    [ReferencedBySourceGenerator]
    public abstract class TychoModule
    {
        private IRequestBroker? _contractFulfillingBroker;
        private IEventBroker? _parentEventBroker;
        private IModuleSettings? _settings;

        /// <summary>
        /// Defines the requests handled and required by the module.
        /// </summary>
        [ReferencedBySourceGenerator]
        protected abstract void DefineContract(IModuleContract module);

        /// <summary>
        /// Defines the events handled and routed by the module.
        /// </summary>
        [ReferencedBySourceGenerator]
        protected abstract void DefineEvents(IModuleEvents module);

        /// <summary>
        /// Defines the submodules used by the module.
        /// </summary>
        [ReferencedBySourceGenerator]
        protected abstract void IncludeModules(IModuleStructure module);

        /// <summary>
        /// Registers services required by the module.
        /// </summary>
        protected abstract void RegisterServices(IServiceCollection module);

        /// <summary>
        /// Creates the internal host builder used by the module.
        /// </summary>
        protected virtual HostApplicationBuilder CreateHostBuilder()
        {
            return Host.CreateEmptyApplicationBuilder(new HostApplicationBuilderSettings
            {
                ApplicationName = GetType().Assembly.GetName().Name,
            });
        }

        /// <summary>
        /// Configures the internal module host and inherits supported context from its parent.
        /// </summary>
        protected virtual void ConfigureHost(IServiceProvider? parentServiceProvider, HostApplicationBuilder moduleHostBuilder)
        {
            moduleHostBuilder.Services.RemoveAll<IHostLifetime>();
            moduleHostBuilder.Services.AddSingleton<IHostLifetime, StandaloneHostLifetime>();

            if (parentServiceProvider == null)
            {
                return;
            }

            IHostEnvironment parentEnvironment = parentServiceProvider.GetRequiredService<IHostEnvironment>();
            moduleHostBuilder.Environment.EnvironmentName = parentEnvironment.EnvironmentName;
            moduleHostBuilder.Environment.ContentRootPath = parentEnvironment.ContentRootPath;

            IFileProvider parentFileProvider = parentEnvironment.ContentRootFileProvider;
            moduleHostBuilder.Environment.ContentRootFileProvider =
                parentFileProvider is NonDisposingFileProvider
                    ? parentFileProvider
                    : new NonDisposingFileProvider(parentFileProvider);

            IConfiguration parentConfiguration = parentServiceProvider.GetRequiredService<IConfiguration>();
            moduleHostBuilder.Configuration.AddConfiguration(parentConfiguration, shouldDisposeConfiguration: false);
            moduleHostBuilder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                [HostDefaults.ApplicationKey] = moduleHostBuilder.Environment.ApplicationName,
                [HostDefaults.EnvironmentKey] = parentEnvironment.EnvironmentName,
                [HostDefaults.ContentRootKey] = parentEnvironment.ContentRootPath,
            });

            ILoggerFactory parentLoggerFactory = parentServiceProvider.GetRequiredService<ILoggerFactory>();
            moduleHostBuilder.Services.AddSingleton(parentLoggerFactory);
        }

        /// <summary>
        /// Runs module startup logic as part of host startup.
        /// </summary>
        protected virtual Task Startup(IServiceProvider module, CancellationToken cancellationToken) => Task.CompletedTask;

        /// <summary>
        /// Runs module cleanup logic as part of host shutdown.
        /// </summary>
        protected virtual Task Cleanup(IServiceProvider module, CancellationToken cancellationToken) => Task.CompletedTask;

        /// <summary>
        /// Retrieves settings supplied by the parent definition.
        /// </summary>
        protected TSettings GetSettings<TSettings>() where TSettings : class, IModuleSettings, new()
        {
            return _settings as TSettings ?? new TSettings();
        }

        internal TychoModule WithSettings(IModuleSettings settings)
        {
            _settings = settings;
            return this;
        }

        internal TychoModule FulfillContract(IRequestBroker contractFulfillingBroker)
        {
            _contractFulfillingBroker = contractFulfillingBroker;
            return this;
        }

        internal TychoModule PassEventBroker(IEventBroker parentEventBroker)
        {
            _parentEventBroker = parentEventBroker;
            return this;
        }

        internal ModuleBuilder CreateModuleBuilder()
        {
            return new ModuleBuilder(GetType())
                .WithHostBuilder(CreateHostBuilder)
                .WithHostConfiguration(ConfigureHost)
                .WithContract(DefineContract, _contractFulfillingBroker)
                .WithEvents(DefineEvents, _parentEventBroker)
                .WithStructure(IncludeModules)
                .WithServices(services =>
                {
                    this.AddGeneratedSetup(services);
                    RegisterServices(services);
                })
                .WithStartup(Startup)
                .WithCleanup(Cleanup);
        }
    }
}
