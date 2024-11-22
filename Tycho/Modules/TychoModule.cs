using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Events.Routing;
using Tycho.Modules.Setup;
using Tycho.Requests.Broker;
using Tycho.Structure;

namespace Tycho.Modules
{
    /// <summary>
    /// Base class for defining a Tycho module
    /// </summary>
    public abstract class TychoModule
    {
        private readonly object _runLock;
        private readonly ModuleBuilder _builder;

        private bool _wasAlreadyRun = false;

        protected IConfiguration Configuration => _builder.Globals.Configuration;

        public TychoModule()
        {
            _runLock = new object();
            _builder = new ModuleBuilder(GetType());
        }

        /// <summary>
        /// Retrieves the settings provided to the module by its parent
        /// </summary>
        /// <typeparam name="TSettings">The type of settings to retrieve</typeparam>
        /// <returns>Matching settings or a new instance of the requested settings type</returns>
        protected TSettings GetSettings<TSettings>() where TSettings : class, IModuleSettings, new()
        {
            return _builder.Settings as TSettings ?? new TSettings();
        }

        /// <summary>
        /// Use this method to define requests handled and required by the module
        /// </summary>
        /// <param name="module">An interface to define the requests</param>
        protected abstract void DefineContract(IModuleContract module);

        /// <summary>
        /// Use this method to define submodules used by the module
        /// </summary>
        /// <param name="module">An interface to define the submodules</param>
        protected abstract void IncludeModules(IModuleStructure module);

        /// <summary>
        /// Use this method to define events handled and routed by the module
        /// </summary>
        /// <param name="module">An interface to define the events</param>
        protected abstract void MapEvents(IModuleEvents module);

        /// <summary>
        /// Use this method to define services required by the module
        /// </summary>
        /// <param name="module">An interface to define the services</param>
        protected abstract void RegisterServices(IServiceCollection module);

        /// <summary>
        /// Override this method if you need to execute code before the module runs
        /// </summary>
        /// <param name="module">A provider of the services configured for the module</param>
        protected virtual Task Startup(IServiceProvider module)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Override this method if you need to execute code before the module is disposed
        /// </summary>
        /// <param name="app">A provider of the services configured for the module</param>
        protected virtual Task Cleanup(IServiceProvider app)
        {
            return Task.CompletedTask;
        }

        internal TychoModule WithGlobals(Globals globals)
        {
            _builder.WithGlobals(globals);
            return this;
        }

        internal TychoModule WithSettings(IModuleSettings settings)
        {
            _builder.WithSettings(settings);
            return this;
        }

        internal TychoModule FulfillContract(IRequestBroker contractFulfillingBroker)
        {
            _builder.WithContractFulfillment(contractFulfillingBroker);
            return this;
        }

        internal TychoModule PassEventRouter(IEventRouter parentEventRouter)
        {
            _builder.WithParentEventRouter(parentEventRouter);
            return this;
        }

        internal async Task<IModule> Run()
        {
            EnsureItIsRunOnlyOnce();

            _builder.WithCleanup(Cleanup).Init();
            RegisterServices(_builder.Services);
            DefineContract(_builder.Contract);
            MapEvents(_builder.Events);
            IncludeModules(_builder.Structure);

            var module = await _builder.Build().ConfigureAwait(false);
            await Startup(module.Internals).ConfigureAwait(false);

            return module;
        }

        private void EnsureItIsRunOnlyOnce()
        {
            lock (_runLock)
            {
                if (_wasAlreadyRun)
                {
                    throw new InvalidOperationException("This module has already been run");
                }
                _wasAlreadyRun = true;
            }
        }
    }
}