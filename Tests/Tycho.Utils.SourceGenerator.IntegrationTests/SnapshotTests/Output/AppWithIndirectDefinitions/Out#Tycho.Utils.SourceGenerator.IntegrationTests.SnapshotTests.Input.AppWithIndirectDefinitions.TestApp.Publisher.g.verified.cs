//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions.TestApp.Publisher.g.cs
namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions
{
    internal class TestAppPublisher : global::Tycho.Events.Publishing.PublisherBase, ITestAppPublisher
    {
        public TestAppPublisher(global::Tycho.Events.Publishing.IEventPublisher genericPublisher) : base(genericPublisher) { }

        public global::System.Threading.Tasks.Task PublishAsync(global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions.TestEventFromHelperExtension eventPayload, global::System.Threading.CancellationToken cancellationToken)
        {
            return PublishAsync<global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions.TestEventFromHelperExtension>(eventPayload, cancellationToken);
        }

        public global::System.Threading.Tasks.Task PublishAsync(global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions.TestEventFromHelperStaticClass eventPayload, global::System.Threading.CancellationToken cancellationToken)
        {
            return PublishAsync<global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions.TestEventFromHelperStaticClass>(eventPayload, cancellationToken);
        }

        public global::System.Threading.Tasks.Task PublishAsync(global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions.TestEventFromHelperClass eventPayload, global::System.Threading.CancellationToken cancellationToken)
        {
            return PublishAsync<global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions.TestEventFromHelperClass>(eventPayload, cancellationToken);
        }

        public global::System.Threading.Tasks.Task PublishAsync(global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions.TestEventFromLocalStaticHelper eventPayload, global::System.Threading.CancellationToken cancellationToken)
        {
            return PublishAsync<global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions.TestEventFromLocalStaticHelper>(eventPayload, cancellationToken);
        }

        public global::System.Threading.Tasks.Task PublishAsync(global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions.TestEventFromLocalHelper eventPayload, global::System.Threading.CancellationToken cancellationToken)
        {
            return PublishAsync<global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions.TestEventFromLocalHelper>(eventPayload, cancellationToken);
        }
    }
}
