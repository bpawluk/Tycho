//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithGenericDefinition.TestApp`1.Publisher.g.cs
namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithGenericDefinition
{
    internal class TestAppPublisher<T> : global::Tycho.Events.Publishing.PublisherBase, ITestAppPublisher<T>
    {
        public TestAppPublisher(global::Tycho.Events.Publishing.IEventPublisher genericPublisher) : base(genericPublisher) { }
    }
}
