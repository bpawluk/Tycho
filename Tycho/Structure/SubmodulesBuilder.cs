using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tycho.Messaging.Contracts;

namespace Tycho.Structure
{
    internal class SubmodulesBuilder : ISubmodulesDefiner
    {
        private HashSet<ModuleDefinition> _submodules;

        public SubmodulesBuilder()
        {
            _submodules = new HashSet<ModuleDefinition>();
        }

        public ISubmodulesDefiner Add<Module>(Action<IOutboxConsumer> contractFullfilment)
            where Module : ModuleDefinition, new()
        {
            var submodule = new Module().Setup(contractFullfilment);
            if (!_submodules.Add(submodule))
            {
                throw new InvalidOperationException("");
            }
            return this;
        }

        internal Task<IEnumerable<IModule>> Build()
        {
            throw new NotImplementedException();
        }
    }
}
