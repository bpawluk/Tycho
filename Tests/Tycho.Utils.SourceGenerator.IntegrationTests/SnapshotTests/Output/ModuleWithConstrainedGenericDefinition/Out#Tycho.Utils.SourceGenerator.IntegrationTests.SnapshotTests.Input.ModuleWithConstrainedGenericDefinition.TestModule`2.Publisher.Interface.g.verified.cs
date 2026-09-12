//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithConstrainedGenericDefinition.TestModule`2.Publisher.Interface.g.cs
namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithConstrainedGenericDefinition
{
    public interface ITestModulePublisher<TPayload, TKey>
        where TPayload : global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithConstrainedGenericDefinition.Model.PayloadBase, global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithConstrainedGenericDefinition.IMarker, new()
        where TKey : notnull
    {
    }
}
