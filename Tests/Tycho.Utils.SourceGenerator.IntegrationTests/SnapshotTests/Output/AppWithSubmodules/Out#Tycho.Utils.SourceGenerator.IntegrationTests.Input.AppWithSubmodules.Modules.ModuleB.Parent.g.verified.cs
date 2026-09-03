//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithSubmodules.Modules.ModuleB.Parent.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Structure.Parent;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithSubmodules.Modules
{
    internal class ModuleBParent : ParentBase, IModuleBParent
    {
        public ModuleBParent(IParentReference parentReference) : base(parentReference) { }
    }
}
