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

        public SubstructureBuilder()
        {
            _submodulesLock = new object();
            _submodules = new HashSet<TychoModule>();
        }

        public ISubstructureDefinition AddSubmodule<Module>(Action<IOutboxConsumer> contractFullfilment)
            where Module : TychoModule, new()
        {
            var submodule = new Module().Setup(contractFullfilment);

            lock (_submodulesLock)
            {
                if (!_submodules.Add(submodule))
                {
                    throw new InvalidOperationException(
                        $"{typeof(Module).Name} is already defined as a submodule for this module");
                }
            }

            return this;
        }

        internal async Task<IEnumerable<IModule>> Build()
        {
            List<TychoModule> submodulesSnapshot;

            lock (_submodulesLock)
            {
                submodulesSnapshot = _submodules.ToList();
            }

            return await Task.WhenAll(submodulesSnapshot.Select(async module => await module.Build().ConfigureAwait(false)));
        }
    }
}
