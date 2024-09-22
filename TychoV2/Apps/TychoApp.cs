using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using TychoV2.Apps.Setup;

namespace TychoV2.Apps
{
    /// <summary>
    /// TODO
    /// </summary>
    public abstract class TychoApp
    {
        private readonly object _runLock = new object();
        private bool _wasAlreadyRun = false;

        private readonly AppBuilder _builder = new AppBuilder();

        /// <summary>
        /// TODO
        /// </summary>
        protected IConfiguration Configuration => _builder.Configuration ?? throw new InvalidOperationException("Configuration not yet available");

        /// <summary>
        /// TODO
        /// </summary>
        protected abstract void DefineContract(IAppContract app);

        /// <summary>
        /// TODO
        /// </summary>
        protected abstract void IncludeModules(IAppStructure app);

        /// <summary>
        /// TODO
        /// </summary>
        protected abstract void MapEvents(IAppEvents app);

        /// <summary>
        /// TODO
        /// </summary>
        protected abstract void RegisterServices(IServiceCollection app);

        /// <summary>
        /// TODO
        /// </summary>
        protected virtual Task Startup(IServiceProvider app) { return Task.CompletedTask; }

        /// <summary>
        /// TODO
        /// </summary>
        public async Task<IApp> Run()
        {
            EnsureItIsRunOnlyOnce();

            RegisterServices(_builder.Services);
            DefineContract(_builder.Contract);
            MapEvents(_builder.Events);
            IncludeModules(_builder.Structure);

            var app = _builder.Build();
            await Startup(app.Services).ConfigureAwait(false);

            return app;
        }

        private void EnsureItIsRunOnlyOnce()
        {
            lock (_runLock)
            {
                if (_wasAlreadyRun) throw new InvalidOperationException("This app has already been run");
                _wasAlreadyRun = true;
            }
        }
    }
}
