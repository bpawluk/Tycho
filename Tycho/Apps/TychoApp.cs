using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Apps.Setup;
using Tycho.Structure;

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
        /// Use this method to define modules used by the application
        /// </summary>
        /// <param name="app">An interface to define the modules</param>
        protected abstract void IncludeModules(IAppStructure app);

        /// <summary>
        /// Use this method to define events handled and routed by the application
        /// </summary>
        /// <param name="app">An interface to define the events</param>
        protected abstract void MapEvents(IAppEvents app);

        /// <summary>
        /// Use this method to define services required by the application
        /// </summary>
        /// <param name="app">An interface to define the services</param>
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
        /// Builds the application according to the definition and runs it
        /// </summary>
        /// <returns>A fresh and ready to use instance of the application</returns>
        /// <exception cref="InvalidOperationException"/>
        public async Task<IApp> Run()
        {
            EnsureItIsRunOnlyOnce();

            _builder.Init();
            RegisterServices(_builder.Services);
            DefineContract(_builder.Contract);
            MapEvents(_builder.Events);
            IncludeModules(_builder.Structure);
            _builder.WithCleanup(Cleanup);

            var app = await _builder.Build().ConfigureAwait(false);
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