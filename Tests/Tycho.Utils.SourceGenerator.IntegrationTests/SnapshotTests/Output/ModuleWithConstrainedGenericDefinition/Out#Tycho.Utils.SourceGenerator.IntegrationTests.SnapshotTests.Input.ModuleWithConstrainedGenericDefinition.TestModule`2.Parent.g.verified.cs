//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithConstrainedGenericDefinition.TestModule`2.Parent.g.cs
namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithConstrainedGenericDefinition
{
    internal class TestModuleParent<TPayload, TKey> : global::Tycho.Structure.Parent.ParentBase, ITestModuleParent<TPayload, TKey>
        where TPayload : global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithConstrainedGenericDefinition.Model.PayloadBase, global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithConstrainedGenericDefinition.IMarker, new()
        where TKey : notnull
    {
        public TestModuleParent(global::Tycho.Structure.Parent.IParentReference parentReference) : base(parentReference) { }
    }
}
