﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using TychoV2.Modules.Setup;

namespace TychoV2.Modules
{
    /// <summary>
    /// TODO
    /// </summary>
    public abstract class TychoModule
    {
        private readonly object _runLock = new object();
        private bool _wasAlreadyRun = false;

        private readonly ModuleBuilder _builder;

        /// <summary>
        /// TODO
        /// </summary>
        protected IConfiguration Configuration => _builder.Configuration ?? throw new InvalidOperationException("Configuration not yet available");

        public TychoModule()
        {
            _builder = new ModuleBuilder(GetType());
        }

        /// <summary>
        /// TODO
        /// </summary>
        protected abstract void DefineContract(IModuleContract module);

        /// <summary>
        /// TODO
        /// </summary>
        protected abstract void IncludeModules(IModuleStructure module);

        /// <summary>
        /// TODO
        /// </summary>
        protected abstract void MapEvents(IModuleEvents module);

        /// <summary>
        /// TODO
        /// </summary>
        protected abstract void RegisterServices(IServiceCollection module);

        /// <summary>
        /// TODO
        /// </summary>
        protected virtual Task Startup(IServiceProvider module) { return Task.CompletedTask; }

        internal TychoModule Configure(Action<IConfigurationBuilder> configurationDefinition)
        {
            _builder.WithConfiguration(configurationDefinition);
            return this;
        }

        internal TychoModule FulfillContract(Action<IContractFulfillment> contractFulfillment, IServiceProvider contractFulfillmentServices)
        {
            _builder.WithContractFulfillment(contractFulfillment, contractFulfillmentServices);
            return this;
        }

        internal async Task<IModule> Run()
        {
            EnsureItIsRunOnlyOnce();

            _builder.Init();
            RegisterServices(_builder.Services);
            DefineContract(_builder.Contract);
            MapEvents(_builder.Events);
            IncludeModules(_builder.Structure);

            var module = _builder.Build();
            await Startup(module.Internals).ConfigureAwait(false);

            return module;
        }

        private void EnsureItIsRunOnlyOnce()
        {
            lock (_runLock)
            {
                if (_wasAlreadyRun) throw new InvalidOperationException("This module has already been run");
                _wasAlreadyRun = true;
            }
        }
    }
}
