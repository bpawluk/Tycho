//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithSubmodules.Modules.Outer`1.Inner.ModuleA.Parent.g.cs
namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithSubmodules.Modules
{
    public partial class Outer<TOuter>
    {
        public partial class Inner
        {
            internal class ModuleAParent : global::Tycho.Structure.Parent.ParentBase, IModuleAParent
            {
                public ModuleAParent(global::Tycho.Structure.Parent.IParentReference parentReference) : base(parentReference) { }
            }
        }
    }
}
