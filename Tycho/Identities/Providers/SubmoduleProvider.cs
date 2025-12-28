using System;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Structure;
using Tycho.Structure.Internal;

namespace Tycho.Identities.Providers
{
    internal class SubmoduleProvider : ISubmoduleProvider
    {
        private readonly Internals _internals;

        public SubmoduleProvider(Internals internals)
        {
            _internals = internals;
        }

        public IModule GetSubmodule(ModuleIdentity moduleIdentity)
        { 
            var submodules = _internals.GetServices<IModule>();
            foreach (var submodule in submodules)
            {
                if (moduleIdentity.MatchesModule(submodule.Internals.Owner))
                {
                    return submodule;
                }
            }
            throw new InvalidOperationException($"Submodule with identity '{moduleIdentity}' not found.");
        }
    }
}
