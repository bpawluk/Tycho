using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using TychoV2.Apps.Instance;
using TychoV2.Structure;

namespace TychoV2.Apps.Setup
{
    internal class AppBuilder
    {
        private readonly Type _appType;
        private readonly Internals _internals;

        private Action<IConfigurationBuilder>? _configurationDefinition;

        public IConfiguration? Configuration { get; private set; }

        public IServiceCollection Services => _internals.GetServiceCollection();

        public AppContract Contract { get; }

        public AppEvents Events { get; } = null!;

        public AppStructure Structure { get; } = null!;

        public AppBuilder(Type appDefinitionType)
        {
            _appType = typeof(App<>).MakeGenericType(new Type[] { appDefinitionType });
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
            var services = _internals.GetServiceCollection();

            var configurationBuilder = new ConfigurationBuilder();
            _configurationDefinition?.Invoke(configurationBuilder);
            Configuration = configurationBuilder.Build();

            services.AddSingleton(Configuration);
            services.AddSingleton(_internals);
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
