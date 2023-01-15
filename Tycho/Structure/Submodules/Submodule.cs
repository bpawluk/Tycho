using Tycho.Structure.Modules;

namespace Tycho.Structure.Submodules
{
    internal class Submodule<Definition> : Module, ISubmodule<Definition>
        where Definition : TychoModule
    {
    }
}
