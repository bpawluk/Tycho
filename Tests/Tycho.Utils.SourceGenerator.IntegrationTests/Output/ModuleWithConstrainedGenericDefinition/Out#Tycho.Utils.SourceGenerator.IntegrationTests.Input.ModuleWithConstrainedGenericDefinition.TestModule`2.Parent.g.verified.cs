//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithConstrainedGenericDefinition.TestModule`2.Parent.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Structure.Parent;
using Tycho.Utils.SourceGenerator.IntegrationTests.Input.SharedConstraints;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithConstrainedGenericDefinition
{
    internal class TestModuleParent<TPayload, TKey> : ParentBase, TestModule<TPayload, TKey>.IParent
        where TPayload : PayloadBase, IMarker, new()
        where TKey : notnull
    {
        public TestModuleParent(IParentReference parentReference) : base(parentReference) { }
    }
}
