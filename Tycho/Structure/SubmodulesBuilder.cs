using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tycho.Messaging.Contracts;

namespace Tycho.Structure
{
    internal class SubmodulesBuilder : ISubmodulesDefiner
    {
        private readonly object _submodulesLock;
        private readonly HashSet<ModuleDefinition> _submodules;

        public SubmodulesBuilder()
        {
            _submodulesLock = new object();
            _submodules = new HashSet<ModuleDefinition>();
        }

        public ISubmodulesDefiner Add<Module>(Action<IOutboxConsumer> contractFullfilment)
            where Module : ModuleDefinition, new()
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
            List<ModuleDefinition> submodulesSnapshot;

            lock (_submodulesLock)
            {
                submodulesSnapshot = _submodules.ToList();
            }

            return await Task.WhenAll(submodulesSnapshot.Select(async module => await module.Build().ConfigureAwait(false)));
        }
    }
}
