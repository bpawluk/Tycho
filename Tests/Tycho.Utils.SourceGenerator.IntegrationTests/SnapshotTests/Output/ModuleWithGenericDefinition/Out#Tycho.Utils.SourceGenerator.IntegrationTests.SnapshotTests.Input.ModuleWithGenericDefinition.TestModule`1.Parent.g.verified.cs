//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithGenericDefinition.TestModule`1.Parent.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Structure.Parent;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithGenericDefinition
{
    internal class TestModuleParent<T> : ParentBase, ITestModuleParent<T>
    {
        public TestModuleParent(IParentReference parentReference) : base(parentReference) { }
    }
}
