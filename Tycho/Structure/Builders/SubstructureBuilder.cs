using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tycho.Contract;

namespace Tycho.Structure.Builders
{
    internal class SubstructureBuilder : ISubstructureDefinition
    {
        private readonly object _submodulesLock;
        private readonly HashSet<TychoModule> _submodules;
        private readonly IServiceProvider _serviceProvider;

        public SubstructureBuilder(IServiceProvider services)
        {
            _submodulesLock = new object();
            _submodules = new HashSet<TychoModule>();
            _serviceProvider = services;
        }

        public ISubstructureDefinition AddSubmodule<Module>() 
            where Module : TychoModule, new()
        {
            var submodule = new Module().UseServices(_serviceProvider);
            AddSubmodule(submodule);
            return this;
        }

        public ISubstructureDefinition AddSubmodule<Module>(Action<IOutboxConsumer> contractFullfilment)
            where Module : TychoModule, new()
        {
            var submodule = new Module().UseServices(_serviceProvider).FulfillContract(contractFullfilment);
            AddSubmodule(submodule);
            return this;
        }
        
        public async Task<IEnumerable<IModule>> Build()
        {
            List<TychoModule> submodulesSnapshot;

            lock (_submodulesLock)
            {
                submodulesSnapshot = _submodules.ToList();
            }

            return await Task.WhenAll(submodulesSnapshot.Select(module => module.Build())).ConfigureAwait(false);
        }

        private void AddSubmodule(TychoModule submodule)
        {
            lock (_submodulesLock)
            {
                if (!_submodules.Add(submodule))
                {
                    throw new InvalidOperationException(submodule.GetType().Name +
                        "is already defined as a submodule for this module");
                }
            }
        }
    }
}
