//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithEvents.TestModule.Publisher.Interface.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Modules;
using Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithEvents.Events;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithEvents
{
    public partial class TestModule : TychoModule
    {
        public interface IPublisher
        {
            Task PublishAsync(OrderCreatedEvent eventPayload, CancellationToken cancellationToken = default);

            Task PublishAsync(PaymentProcessedEvent eventPayload, CancellationToken cancellationToken = default);

            Task PublishAsync(PaymentFailedEvent eventPayload, CancellationToken cancellationToken = default);
        }
    }
}
