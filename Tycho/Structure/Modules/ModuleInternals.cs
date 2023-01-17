using System;
using System.Collections.Generic;

namespace Tycho.Structure.Modules
{
    internal class ModuleInternals : ModuleBase, IModule
    {
        private IReadOnlyDictionary<Type, IModule>? _submodules;

        public IModule GetSubmodule<Definition>() where Definition : TychoModule
        {
            if (_submodules?.ContainsKey(typeof(Definition)) is true)
            {
                return _submodules[typeof(Definition)];
            }
            throw new InvalidOperationException($"{typeof(Definition).Name} is not a submodule of this module");
        }

        public void SetSubmodules(IEnumerable<IModule> submodules)
        {
            if (_submodules is null)
            {
                var submodulesCollection = new Dictionary<Type, IModule>();

                foreach (var submodule in submodules)
                {
                    var submoduleDefinitionType = submodule.GetType().GetGenericArguments()[0];
                    if (!submodulesCollection.TryAdd(submoduleDefinitionType, submodule))
                    {
                        throw new InvalidOperationException(submoduleDefinitionType.Name +
                            " is already defined as a submodule of this module");
                    }
                }

                _submodules = submodulesCollection;
            }
            else
            {
                throw new InvalidOperationException("Submodules of this module are already defined");
            }
        }
    }
}
