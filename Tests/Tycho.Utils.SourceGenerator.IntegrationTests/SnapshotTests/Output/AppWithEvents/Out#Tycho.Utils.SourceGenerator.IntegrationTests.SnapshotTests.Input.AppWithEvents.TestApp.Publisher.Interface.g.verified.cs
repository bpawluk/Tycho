//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithEvents.TestApp.Publisher.Interface.g.cs
namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithEvents
{
    public interface ITestAppPublisher
    {
        global::System.Threading.Tasks.Task PublishAsync(global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithEvents.Events.OrderCreatedEvent eventPayload, global::System.Threading.CancellationToken cancellationToken = default);

        global::System.Threading.Tasks.Task PublishAsync(global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithEvents.Events.PaymentProcessedEvent eventPayload, global::System.Threading.CancellationToken cancellationToken = default);

        global::System.Threading.Tasks.Task PublishAsync(global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithEvents.Events.PaymentFailedEvent eventPayload, global::System.Threading.CancellationToken cancellationToken = default);
    }
}
