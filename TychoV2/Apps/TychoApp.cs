using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using TychoV2.Apps.Setup;

namespace TychoV2.Apps
{
    public abstract class TychoApp
    {
        private readonly object _runLock = new object();
        private bool _wasAlreadyRun = false;

        private readonly AppBuilder _builder = new AppBuilder();

        protected IConfiguration Configuration => _builder.Configuration ?? throw new InvalidOperationException("Configuration not yet available");

        protected abstract void DefineContract(IAppContract app);

        protected abstract void IncludeModules(IAppStructure app);

        protected abstract void MapEvents(IAppEvents app);

        protected abstract void RegisterServices(IServiceCollection app);

        protected virtual Task Startup(IServiceProvider app) { return Task.CompletedTask; }

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
