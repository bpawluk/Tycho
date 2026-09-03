//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithEvents.TestModule.Publisher.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Publishing;
using Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithEvents.Events;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithEvents
{
    internal class TestModulePublisher : PublisherBase, ITestModulePublisher
    {
        public TestModulePublisher(IEventPublisher genericPublisher) : base(genericPublisher) { }

        public Task PublishAsync(OrderCreatedEvent eventPayload, CancellationToken cancellationToken)
        {
            return PublishAsync<OrderCreatedEvent>(eventPayload, cancellationToken);
        }

        public Task PublishAsync(PaymentProcessedEvent eventPayload, CancellationToken cancellationToken)
        {
            return PublishAsync<PaymentProcessedEvent>(eventPayload, cancellationToken);
        }

        public Task PublishAsync(PaymentFailedEvent eventPayload, CancellationToken cancellationToken)
        {
            return PublishAsync<PaymentFailedEvent>(eventPayload, cancellationToken);
        }
    }
}
