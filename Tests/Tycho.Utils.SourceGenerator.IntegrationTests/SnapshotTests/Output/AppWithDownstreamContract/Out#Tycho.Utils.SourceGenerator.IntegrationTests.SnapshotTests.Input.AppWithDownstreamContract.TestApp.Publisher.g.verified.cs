//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithDownstreamContract.TestApp.Publisher.g.cs
namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithDownstreamContract
{
    internal class TestAppPublisher : global::Tycho.Events.Publishing.PublisherBase, ITestAppPublisher
    {
        public TestAppPublisher(global::Tycho.Events.Publishing.IEventPublisher genericPublisher) : base(genericPublisher) { }
    }
}
