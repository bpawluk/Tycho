using Tycho.Structure;

namespace Tycho.Identities.Providers
{
    internal interface ISubmoduleProvider
    {
        IModule GetSubmodule(ModuleIdentity moduleIdentity);
    }
}
