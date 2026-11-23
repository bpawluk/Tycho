//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithEvents.TestModule.Publisher.g.cs
namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithEvents
{
    internal class TestModulePublisher : global::Tycho.Events.Publishing.PublisherBase, ITestModulePublisher
    {
        public TestModulePublisher(global::Tycho.Events.Publishing.IEventPublisher genericPublisher) : base(genericPublisher) { }

        public global::System.Threading.Tasks.Task PublishAsync(global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithEvents.Events.OrderCreatedEvent eventPayload, global::System.Threading.CancellationToken cancellationToken)
        {
            return PublishAsync<global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithEvents.Events.OrderCreatedEvent>(eventPayload, cancellationToken);
        }

        public global::System.Threading.Tasks.Task PublishAsync(global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithEvents.Events.PaymentProcessedEvent eventPayload, global::System.Threading.CancellationToken cancellationToken)
        {
            return PublishAsync<global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithEvents.Events.PaymentProcessedEvent>(eventPayload, cancellationToken);
        }

        public global::System.Threading.Tasks.Task PublishAsync(global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithEvents.Events.PaymentFailedEvent eventPayload, global::System.Threading.CancellationToken cancellationToken)
        {
            return PublishAsync<global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithEvents.Events.PaymentFailedEvent>(eventPayload, cancellationToken);
        }
    }
}
