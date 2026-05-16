using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tycho.Apps.Instance;
using Tycho.Structure;

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
            if (Globals.LoggingSetup != null)
            {
                Services.AddLogging(Globals.LoggingSetup);
            }
            Services.AddSingleton(_internals);

            return this;
        }

        public async Task<IApp> BuildAsync()
        {
            IApp app = Activator.CreateInstance(_appType, _internals, _cleanup) as IApp ?? throw new InvalidOperationException($"Failed to create an instance of {_appType.Name}.");
            await Contract.BuildAsync().ConfigureAwait(false);
            await Events.BuildAsync().ConfigureAwait(false);
            await Structure.BuildAsync().ConfigureAwait(false);
            _internals.Build();

            return app;
        }
    }
}
