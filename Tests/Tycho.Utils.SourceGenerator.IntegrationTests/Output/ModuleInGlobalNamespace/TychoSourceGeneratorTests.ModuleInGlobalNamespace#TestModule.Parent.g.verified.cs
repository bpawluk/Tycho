//HintName: TestModule.Parent.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Structure.Parent;

    internal class TestModuleParent : ParentBase, TestModule.IParent
    {
        public TestModuleParent(IParentReference parentReference) : base(parentReference) { }
    }
