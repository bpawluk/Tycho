//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithConstrainedGenericDefinition.TestModule`2.Publisher.g.cs
namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithConstrainedGenericDefinition
{
    internal class TestModulePublisher<TPayload, TKey> : global::Tycho.Events.Publishing.PublisherBase, ITestModulePublisher<TPayload, TKey>
        where TPayload : global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithConstrainedGenericDefinition.Model.PayloadBase, global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithConstrainedGenericDefinition.IMarker, new()
        where TKey : notnull
    {
        public TestModulePublisher(global::Tycho.Events.Publishing.IEventPublisher genericPublisher) : base(genericPublisher) { }
    }
}
