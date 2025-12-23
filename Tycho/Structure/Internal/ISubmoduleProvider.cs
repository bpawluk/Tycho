using Tycho.Identities;

namespace Tycho.Structure.Internal
{
    internal interface ISubmoduleProvider
    {
        IModule GetSubmodule(ModuleIdentity moduleIdentity);
    }
}
