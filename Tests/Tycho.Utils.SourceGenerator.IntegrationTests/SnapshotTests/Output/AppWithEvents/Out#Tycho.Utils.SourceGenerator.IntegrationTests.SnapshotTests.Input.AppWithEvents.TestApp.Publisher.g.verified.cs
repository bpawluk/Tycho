//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithEvents.TestApp.Publisher.g.cs
namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithEvents
{
    internal class TestAppPublisher : global::Tycho.Events.Publishing.PublisherBase, ITestAppPublisher
    {
        public TestAppPublisher(global::Tycho.Events.Publishing.IEventPublisher genericPublisher) : base(genericPublisher) { }

        public global::System.Threading.Tasks.Task PublishAsync(global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithEvents.Events.OrderCreatedEvent eventPayload, global::System.Threading.CancellationToken cancellationToken)
        {
            return PublishAsync<global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithEvents.Events.OrderCreatedEvent>(eventPayload, cancellationToken);
        }

        public global::System.Threading.Tasks.Task PublishAsync(global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithEvents.Events.PaymentProcessedEvent eventPayload, global::System.Threading.CancellationToken cancellationToken)
        {
            return PublishAsync<global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithEvents.Events.PaymentProcessedEvent>(eventPayload, cancellationToken);
        }

        public global::System.Threading.Tasks.Task PublishAsync(global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithEvents.Events.PaymentFailedEvent eventPayload, global::System.Threading.CancellationToken cancellationToken)
        {
            return PublishAsync<global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithEvents.Events.PaymentFailedEvent>(eventPayload, cancellationToken);
        }
    }
}
