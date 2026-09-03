//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithEvents.TestApp.Publisher.Interface.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithEvents.Events;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithEvents
{
    public interface ITestAppPublisher
    {
        Task PublishAsync(OrderCreatedEvent eventPayload, CancellationToken cancellationToken = default);

        Task PublishAsync(PaymentProcessedEvent eventPayload, CancellationToken cancellationToken = default);

        Task PublishAsync(PaymentFailedEvent eventPayload, CancellationToken cancellationToken = default);
    }
}
