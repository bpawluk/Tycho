//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithEvents.TestApp.Publisher.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Publishing;
using Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithEvents.Events;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithEvents
{
    internal class TestAppPublisher : PublisherBase, ITestAppPublisher
    {
        public TestAppPublisher(IEventPublisher genericPublisher) : base(genericPublisher) { }

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
