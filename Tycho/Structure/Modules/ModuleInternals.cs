using System;
using System.Collections.Generic;
using System.Linq;

namespace Tycho.Structure.Modules
{
    internal class ModuleInternals : ModuleBase, IModule
    {
        private IReadOnlyDictionary<Type, IModule>? _submodules;

        public IModule GetSubmodule<T>()
        {
            if (_submodules?.ContainsKey(typeof(T)) is true)
            {
                return _submodules[typeof(T)];
            }
            throw new InvalidOperationException($"{typeof(T).Name} is not a submodule of this module");
        }

        public void SetSubmodules(IEnumerable<IModule> submodules)
        {
            if (_submodules is null)
            {
                _submodules = submodules.ToDictionary(x => x.GetType());
            }
            else
            {
                throw new InvalidOperationException("Submodules of this module are already defined");
            }
        }
    }
}
