//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithConstrainedGenericDefinition.TestApp`2.Publisher.g.cs
namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithConstrainedGenericDefinition
{
    internal class TestAppPublisher<TPayload, TKey> : global::Tycho.Events.Publishing.PublisherBase, ITestAppPublisher<TPayload, TKey>
        where TPayload : global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithConstrainedGenericDefinition.Model.PayloadBase, global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithConstrainedGenericDefinition.IMarker, new()
        where TKey : notnull
    {
        public TestAppPublisher(global::Tycho.Events.Publishing.IEventPublisher genericPublisher) : base(genericPublisher) { }
    }
}
