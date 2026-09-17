//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithEvents.TestModule.Publisher.Interface.g.cs
namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithEvents
{
    public interface ITestModulePublisher
    {
        global::System.Threading.Tasks.Task PublishAsync(global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithEvents.Events.OrderCreatedEvent eventPayload, global::System.Threading.CancellationToken cancellationToken = default);

        global::System.Threading.Tasks.Task PublishAsync(global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithEvents.Events.PaymentProcessedEvent eventPayload, global::System.Threading.CancellationToken cancellationToken = default);

        global::System.Threading.Tasks.Task PublishAsync(global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithEvents.Events.PaymentFailedEvent eventPayload, global::System.Threading.CancellationToken cancellationToken = default);
    }
}
