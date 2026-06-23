using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tycho.Apps.Instance;
using Tycho.Apps.Setup;
using Tycho.Utils;

namespace Tycho.Apps
{
    /// <summary>
    /// Base class for defining a Tycho Application
    /// </summary>
    [ReferencedBySourceGenerator]
    public abstract class TychoApp
    {
        private readonly object _runLock;
        private readonly AppBuilder _builder;

        private bool _wasAlreadyRun = false;

        /// <summary>
        /// Gets the global configuration used by the Application and its Modules.
        /// </summary>
        protected IConfiguration Configuration => _builder.Globals.Configuration;

        /// <summary>
        /// Creates a new instance of the <see cref="TychoApp"/> class.
        /// </summary>
        public TychoApp()
        {
            _runLock = new object();
            _builder = new AppBuilder(GetType());
        }

        /// <summary>
        /// Use this method to define Requests handled by the Application
        /// </summary>
        /// <param name="app">An interface to define Requests</param>
        [ReferencedBySourceGenerator]
        protected abstract void DefineContract(IAppContract app);

        /// <summary>
        /// Use this method to define Events handled and routed by the Application
        /// </summary>
        /// <param name="app">An interface to define Events</param>
        [ReferencedBySourceGenerator]
        protected abstract void DefineEvents(IAppEvents app);

        /// <summary>
        /// Use this method to define Modules used by the Application
        /// </summary>
        /// <param name="app">An interface to include Modules</param>
        [ReferencedBySourceGenerator]
        protected abstract void IncludeModules(IAppStructure app);

        /// <summary>
        /// Use this method to define services required by the Application
        /// </summary>
        /// <param name="app">An interface to register services</param>
        protected abstract void RegisterServices(IServiceCollection app);

        /// <summary>
        /// Override this method if you need to execute code before the Application runs
        /// </summary>
        /// <param name="app">A provider of the services configured for the Application</param>
        protected virtual Task Startup(IServiceProvider app)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Override this method if you need to execute code before the Application is disposed
        /// </summary>
        /// <param name="app">A provider of the services configured for the Application</param>
        protected virtual Task Cleanup(IServiceProvider app)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Provides automated setup for the App.
        /// </summary>
        /// <remarks>
        /// Do not override – it is implemented using source generation.
        /// </remarks>
#pragma warning disable IDE1006
        [ReferencedBySourceGenerator]
        protected virtual void __AutoSetup__(IServiceCollection app)
        {
            throw new NotImplementedException(
                $"Failed to provide automated setup for {GetType()} app. " +
                $"Make sure your app definition is a public partial class marked with the TychoDefinition attribute");
        }
#pragma warning restore IDE1006

        /// <summary>
        /// Supplies global configuration for the Application and its Modules.
        /// </summary>
        /// <param name="globalConfiguration">Configuration to be used</param>
        /// <exception cref="ArgumentNullException"/>"
        [ReferencedBySourceGenerator]
        public void WithConfigurationBase(IConfiguration globalConfiguration)
        {
            globalConfiguration.ThrowIfNull();
            _builder.WithConfiguration(globalConfiguration);
        }

        /// <summary>
        /// Supplies logging setup for the Application and its Modules.
        /// </summary>
        /// <param name="loggingSetup">Logging setup to be used</param>
        /// <exception cref="ArgumentNullException"/>"
        [ReferencedBySourceGenerator]
        public void WithLoggingBase(Action<ILoggingBuilder> loggingSetup)
        {
            loggingSetup.ThrowIfNull();
            _builder.WithLogging(loggingSetup);
        }

        /// <summary>
        /// Builds and runs the Application according to the definition.
        /// </summary>
        /// <returns>A fresh and ready to use instance of the Application</returns>
        /// <exception cref="InvalidOperationException"/>
        [ReferencedBySourceGenerator]
        public async Task<IApp> RunBaseAsync()
        {
            EnsureItIsRunOnlyOnce();

            _builder.WithCleanup(Cleanup).Init();
            this.AddGeneratedSetup(_builder.Services);

            RegisterServices(_builder.Services);
            DefineContract(_builder.Contract);
            DefineEvents(_builder.Events);
            IncludeModules(_builder.Structure);

            IApp app = await _builder.BuildAsync().ConfigureAwait(false);
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
