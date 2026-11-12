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
using Tycho.Apps.Setup;
using Tycho.Hosting;
using Tycho.Hosting.Files;
using Tycho.Utils;

namespace Tycho.Apps
{
    /// <summary>
    /// Base class for defining a Tycho application.
    /// </summary>
    [ReferencedBySourceGenerator]
    public abstract class TychoApp
    {
        /// <summary>
        /// Defines the requests handled by the application.
        /// </summary>
        [ReferencedBySourceGenerator]
        protected abstract void DefineContract(IAppContract app);

        /// <summary>
        /// Defines the events handled and routed by the application.
        /// </summary>
        [ReferencedBySourceGenerator]
        protected abstract void DefineEvents(IAppEvents app);

        /// <summary>
        /// Defines the modules used by the application.
        /// </summary>
        [ReferencedBySourceGenerator]
        protected abstract void IncludeModules(IAppStructure app);

        /// <summary>
        /// Registers services required by the application.
        /// </summary>
        protected abstract void RegisterServices(IServiceCollection app);

        /// <summary>
        /// Creates the internal host builder used by the application.
        /// </summary>
        protected virtual HostApplicationBuilder CreateHostBuilder()
        {
            return Host.CreateEmptyApplicationBuilder(new HostApplicationBuilderSettings
            {
                ApplicationName = GetType().Assembly.GetName().Name,
            });
        }

        /// <summary>
        /// Configures the internal application host.
        /// </summary>
        protected virtual void ConfigureHost(IServiceProvider? parentServiceProvider, HostApplicationBuilder appHostBuilder)
        {
            appHostBuilder.Services.RemoveAll<IHostLifetime>();
            appHostBuilder.Services.AddSingleton<IHostLifetime, StandaloneHostLifetime>();

            if (parentServiceProvider == null)
            {
                return;
            }

            IHostEnvironment parentEnvironment = parentServiceProvider.GetRequiredService<IHostEnvironment>();
            appHostBuilder.Environment.EnvironmentName = parentEnvironment.EnvironmentName;
            appHostBuilder.Environment.ContentRootPath = parentEnvironment.ContentRootPath;

            IFileProvider parentFileProvider = parentEnvironment.ContentRootFileProvider;
            appHostBuilder.Environment.ContentRootFileProvider =
                parentFileProvider is NonDisposingFileProvider
                    ? parentFileProvider
                    : new NonDisposingFileProvider(parentFileProvider);

            IConfiguration parentConfiguration = parentServiceProvider.GetRequiredService<IConfiguration>();
            appHostBuilder.Configuration.AddConfiguration(parentConfiguration, shouldDisposeConfiguration: false);
            appHostBuilder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                [HostDefaults.ApplicationKey] = appHostBuilder.Environment.ApplicationName,
                [HostDefaults.EnvironmentKey] = appHostBuilder.Environment.EnvironmentName,
                [HostDefaults.ContentRootKey] = appHostBuilder.Environment.ContentRootPath,
            });

            ILoggerFactory parentLoggerFactory = parentServiceProvider.GetRequiredService<ILoggerFactory>();
            appHostBuilder.Services.AddSingleton(parentLoggerFactory);
        }

        /// <summary>
        /// Runs application startup logic.
        /// </summary>
        protected virtual Task Startup(IServiceProvider app, CancellationToken cancellationToken) => Task.CompletedTask;

        /// <summary>
        /// Runs application cleanup logic.
        /// </summary>
        protected virtual Task Cleanup(IServiceProvider app, CancellationToken cancellationToken) => Task.CompletedTask;

        /// <summary>
        /// Creates the base application builder.
        /// </summary>
        [ReferencedBySourceGenerator]
        public IAppBuilderBase CreateAppBuilderBase()
        {
            return new AppBuilderBase(GetType())
                .WithHostBuilder(CreateHostBuilder)
                .WithHostConfiguration(ConfigureHost)
                .WithContract(DefineContract)
                .WithEvents(DefineEvents)
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
