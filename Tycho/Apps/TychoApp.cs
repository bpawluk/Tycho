using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tycho.Apps.Setup;
using Tycho.Structure;
using Tycho.Utils;

namespace Tycho.Apps
{
    /// <summary>
    /// Base class for defining a Tycho application
    /// </summary>
    public abstract class TychoApp
    {
        private readonly object _runLock;
        private readonly AppBuilder _builder;

        private bool _wasAlreadyRun = false;

        /// <summary>
        /// Gets the global configuration used by the application and its modules.
        /// </summary>
        protected IConfiguration Configuration => _builder.Globals.Configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="TychoApp"/> class.
        /// </summary>
        public TychoApp()
        {
            _runLock = new object();
            _builder = new AppBuilder(GetType());
        }

        /// <summary>
        /// Use this method to define requests handled by the application
        /// </summary>
        /// <param name="app">An interface to define the requests</param>
        protected abstract void DefineContract(IAppContract app);

        /// <summary>
        /// Use this method to define events handled and routed by the application
        /// </summary>
        /// <param name="app">An interface to define the events</param>
        protected abstract void DefineEvents(IAppEvents app);

        /// <summary>
        /// Use this method to define modules used by the application
        /// </summary>
        /// <param name="app">An interface to include the modules</param>
        protected abstract void IncludeModules(IAppStructure app);

        /// <summary>
        /// Use this method to define services required by the application
        /// </summary>
        /// <param name="app">An interface to register the services</param>
        protected abstract void RegisterServices(IServiceCollection app);

        /// <summary>
        /// Override this method if you need to execute code before the application runs
        /// </summary>
        /// <param name="app">A provider of the services configured for the application</param>
        protected virtual Task Startup(IServiceProvider app)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Override this method if you need to execute code before the application is disposed
        /// </summary>
        /// <param name="app">A provider of the services configured for the application</param>
        protected virtual Task Cleanup(IServiceProvider app)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Provides automated setup for the app.
        /// </summary>
        /// <remarks>
        /// Do not override – it is implemented using source generation.
        /// </remarks>
#pragma warning disable IDE1006
        protected virtual void __AutoSetup__(IServiceCollection app)
        {
            throw new NotImplementedException(
                $"Failed to provide automated setup for {GetType()} app. " +
                $"Make sure your app definition is a public partial class marked with the TychoDefinition attribute");
        }
#pragma warning restore IDE1006

        /// <summary>
        /// Supplies global configuration for the application and its modules.
        /// </summary>
        /// <param name="globalConfiguration">Configuration to be used</param>
        /// <exception cref="ArgumentNullException"/>"
        protected void WithConfigurationBase(IConfiguration globalConfiguration)
        {
            globalConfiguration.ThrowIfNull();
            _builder.WithConfiguration(globalConfiguration);
        }

        /// <summary>
        /// Supplies logging setup for the application and its modules.
        /// </summary>
        /// <param name="loggingSetup">Logging setup to be used</param>
        /// <exception cref="ArgumentNullException"/>"
        protected void WithLoggingBase(Action<ILoggingBuilder> loggingSetup)
        {
            loggingSetup.ThrowIfNull();
            _builder.WithLogging(loggingSetup);
        }

        /// <summary>
        /// Builds and runs the application according to the definition.
        /// </summary>
        /// <returns>A fresh and ready to use instance of the application</returns>
        /// <exception cref="InvalidOperationException"/>
        protected async Task<IAppInstance> RunBaseAsync()
        {
            EnsureItIsRunOnlyOnce();

            _builder.WithCleanup(Cleanup).Init();
            __AutoSetup__(_builder.Services);
            RegisterServices(_builder.Services);
            DefineContract(_builder.Contract);
            DefineEvents(_builder.Events);
            IncludeModules(_builder.Structure);

            var app = await _builder.BuildAsync().ConfigureAwait(false);
            await Startup(app.Internals).ConfigureAwait(false);

            return app;
        }

        private void EnsureItIsRunOnlyOnce()
        {
            lock (_runLock)
            {
                if (_wasAlreadyRun)
                {
                    throw new InvalidOperationException("This app has already been run");
                }
                _wasAlreadyRun = true;
            }
        }
    }
}
