//HintName: Outer.Inner.TestModule.Parent.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Structure.Parent;

public partial class Outer
{
    public partial class Inner
    {
        internal class TestModuleParent : ParentBase, TestModule.IParent
        {
            public TestModuleParent(IParentReference parentReference) : base(parentReference) { }
        }
    }
}
