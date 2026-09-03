//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithDownstreamContract.TestModule.Parent.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Structure.Parent;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithDownstreamContract
{
    internal class TestModuleParent : ParentBase, ITestModuleParent
    {
        public TestModuleParent(IParentReference parentReference) : base(parentReference) { }
    }
}
