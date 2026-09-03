//HintName: ModuleOuter.ModuleInner.TestModule.Parent.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Structure.Parent;

public partial class ModuleOuter
{
    public partial class ModuleInner
    {
        internal class TestModuleParent : ParentBase, ITestModuleParent
        {
            public TestModuleParent(IParentReference parentReference) : base(parentReference) { }
        }
    }
}
