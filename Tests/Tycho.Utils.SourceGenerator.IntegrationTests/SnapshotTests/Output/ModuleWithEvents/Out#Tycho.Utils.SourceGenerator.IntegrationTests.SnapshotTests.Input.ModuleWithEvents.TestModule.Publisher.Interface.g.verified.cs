//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithEvents.TestModule.Publisher.Interface.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithEvents.Events;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithEvents
{
    public interface ITestModulePublisher
    {
        Task PublishAsync(OrderCreatedEvent eventPayload, CancellationToken cancellationToken = default);

        Task PublishAsync(PaymentProcessedEvent eventPayload, CancellationToken cancellationToken = default);

        Task PublishAsync(PaymentFailedEvent eventPayload, CancellationToken cancellationToken = default);
    }
}
