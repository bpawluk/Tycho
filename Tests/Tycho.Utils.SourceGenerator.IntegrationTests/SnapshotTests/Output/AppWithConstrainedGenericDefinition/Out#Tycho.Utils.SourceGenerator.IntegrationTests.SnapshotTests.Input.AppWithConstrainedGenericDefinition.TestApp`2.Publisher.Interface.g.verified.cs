//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithConstrainedGenericDefinition.TestApp`2.Publisher.Interface.g.cs
namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithConstrainedGenericDefinition
{
    public interface ITestAppPublisher<TPayload, TKey>
        where TPayload : global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithConstrainedGenericDefinition.Model.PayloadBase, global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithConstrainedGenericDefinition.IMarker, new()
        where TKey : notnull
    {
    }
}
