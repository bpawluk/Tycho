//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithSubmodules.TestModule.Parent.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Structure.Parent;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithSubmodules
{
    internal class TestModuleParent : ParentBase, TestModule.IParent
    {
        public TestModuleParent(IParentReference parentReference) : base(parentReference) { }
    }
}
