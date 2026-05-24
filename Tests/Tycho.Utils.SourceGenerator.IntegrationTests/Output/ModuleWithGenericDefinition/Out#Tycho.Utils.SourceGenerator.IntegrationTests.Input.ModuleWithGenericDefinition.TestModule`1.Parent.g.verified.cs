//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithGenericDefinition.TestModule`1.Parent.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Structure.Parent;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithGenericDefinition
{
    internal class TestModuleParent<T> : ParentBase, TestModule<T>.IParent
    {
        public TestModuleParent(IParentReference parentReference) : base(parentReference) { }
    }
}
