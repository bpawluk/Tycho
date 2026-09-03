//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleInNamespace.TestModule.Parent.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Structure.Parent;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleInNamespace
{
    internal class TestModuleParent : ParentBase, ITestModuleParent
    {
        public TestModuleParent(IParentReference parentReference) : base(parentReference) { }
    }
}
