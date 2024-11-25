using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tycho.Apps.Instance;
using Tycho.Structure;
using Tycho.Structure.Data;

namespace Tycho.Apps.Setup
{
    internal class AppBuilder
    {
        private readonly Type _appType;
        private readonly Internals _internals;

        private Func<IServiceProvider, Task>? _cleanup;

        public Globals Globals { get; }

        public AppContract Contract { get; }

        public AppEvents Events { get; }

        public AppStructure Structure { get; }

        public IServiceCollection Services => _internals.GetServiceCollection();

        public AppBuilder(Type appDefinitionType)
        {
            _appType = typeof(App<>).MakeGenericType(appDefinitionType);
            _internals = new Internals(appDefinitionType);
            Globals = new Globals();
            Contract = new AppContract(_internals);
            Events = new AppEvents(_internals);
            Structure = new AppStructure(_internals, Globals);
        }

        public AppBuilder WithConfiguration(IConfiguration globalConfiguration)
        {
            Globals.Configuration = globalConfiguration;
            return this;
        }

        public AppBuilder WithLogging(Action<ILoggingBuilder> loggingSetup)
        {
            Globals.LoggingSetup = loggingSetup;
            return this;
        }

        public AppBuilder WithCleanup(Func<IServiceProvider, Task> cleanup)
        {
            _cleanup = cleanup;
            return this;
        }

        public AppBuilder Init()
        {
            var services = _internals.GetServiceCollection();

            if (Globals.LoggingSetup != null)
            {
                services.AddLogging(Globals.LoggingSetup);
            }
            services.AddSingleton(_internals);

            return this;
        }

        public async Task<IApp> Build()
        {
            var app = (IApp)Activator.CreateInstance(_appType, _internals, _cleanup);

            await Contract.Build().ConfigureAwait(false);
            await Events.Build().ConfigureAwait(false);
            await Structure.Build().ConfigureAwait(false);
            _internals.Build();

            return app;
        }
    }
}