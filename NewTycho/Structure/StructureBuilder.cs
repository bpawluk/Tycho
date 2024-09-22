using Microsoft.Extensions.Configuration;
using NewTycho.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewTycho.Structure
{
    internal class StructureBuilder : IStructureDefinition
    {
        private readonly HashSet<TychoModule> _modules;
        private readonly IServiceProvider _serviceProvider;

        public StructureBuilder(IServiceProvider services)
        {
            _modules = new HashSet<TychoModule>();
            _serviceProvider = services;
        }

        IStructureDefinition IStructureDefinition.AddModule<TModule>(Action<IOutboxConsumer> contractFulfillment)
        {
            return Add<TModule>(contractFulfillment);
        }

        IStructureDefinition IStructureDefinition.AddModule<TModule>(Action<IConfigurationBuilder> configurationDefinition)
        {
            return Add<TModule>(configurationDefinition: configurationDefinition);
        }

        IStructureDefinition IStructureDefinition.AddModule<TModule>(
            Action<IOutboxConsumer> contractFulfillment, 
            Action<IConfigurationBuilder> configurationDefinition)
        {
            return Add<TModule>(contractFulfillment, configurationDefinition);
        }

        public async Task<IEnumerable<Module>> Build()
        {
            return await Task.WhenAll(_modules.Select(module => module.Build())).ConfigureAwait(false);
        }

        private IStructureDefinition Add<TModule>(
            Action<IOutboxConsumer>? contractFulfillment = null,
            Action<IConfigurationBuilder>? configurationDefinition = null)
            where TModule : TychoModule, new()
        {
            var submodule = new TModule();

            if (contractFulfillment != null)
            {
                submodule.FulfillContract(contractFulfillment, _serviceProvider);
            }

            if (configurationDefinition != null)
            {
                submodule.Configure(configurationDefinition);
            }

            if (!_modules.Add(submodule))
            {
                throw new InvalidOperationException($"{typeof(TModule).Name} is already defined as a submodule for this module");
            }

            return this;
        }
    }
}
