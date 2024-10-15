using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Apps.Instance;
using Tycho.Structure;

namespace Tycho.Apps.Setup
{
    internal class AppBuilder
    {
        private readonly Type _appType;
        private readonly Internals _internals;

        private Action<IConfigurationBuilder>? _configurationDefinition;

        public IConfiguration? Configuration { get; private set; }

        public IServiceCollection Services => _internals.GetServiceCollection();

        public AppContract Contract { get; }

        public AppEvents Events { get; }

        public AppStructure Structure { get; }

        public AppBuilder(Type appDefinitionType)
        {
            _appType = typeof(App<>).MakeGenericType(appDefinitionType);
            _internals = new Internals(appDefinitionType);
            Contract = new AppContract(_internals);
            Events = new AppEvents(_internals);
            Structure = new AppStructure(_internals);
        }

        public void WithConfiguration(Action<IConfigurationBuilder> configurationDefinition)
        {
            _configurationDefinition = configurationDefinition;
        }

        public void Init()
        {
            var configurationBuilder = new ConfigurationBuilder();
            _configurationDefinition?.Invoke(configurationBuilder);
            Configuration = configurationBuilder.Build();

            _internals.GetServiceCollection()
                .AddSingleton(Configuration)
                .AddSingleton(_internals);
        }

        public async Task<IApp> Build()
        {
            var app = (IApp)Activator.CreateInstance(_appType, _internals);

            await Contract.Build();
            await Events.Build();
            await Structure.Build();
            _internals.Build();

            return app;
        }
    }
}