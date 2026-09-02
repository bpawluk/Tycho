//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithSubmodules.Modules.Outer`1.Inner.ModuleA.Parent.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Structure.Parent;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithSubmodules.Modules
{
    public partial class Outer<TOuter>
    {
        public partial class Inner
        {
            internal class ModuleAParent : ParentBase, IModuleAParent
            {
                public ModuleAParent(IParentReference parentReference) : base(parentReference) { }
            }
        }
    }
}
