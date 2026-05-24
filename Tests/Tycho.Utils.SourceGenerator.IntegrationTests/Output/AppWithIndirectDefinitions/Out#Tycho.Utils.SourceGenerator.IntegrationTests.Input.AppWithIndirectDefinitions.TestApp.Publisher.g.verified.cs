//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithIndirectDefinitions.TestApp.Publisher.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Publishing;
using Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithIndirectDefinitions;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithIndirectDefinitions
{
    internal class TestAppPublisher : PublisherBase, TestApp.IPublisher
    {
        public TestAppPublisher(IEventPublisher genericPublisher) : base(genericPublisher) { }

        public Task PublishAsync(TestEventFromHelperExtension eventPayload, CancellationToken cancellationToken)
        {
            return PublishAsync<TestEventFromHelperExtension>(eventPayload, cancellationToken);
        }

        public Task PublishAsync(TestEventFromHelperStaticClass eventPayload, CancellationToken cancellationToken)
        {
            return PublishAsync<TestEventFromHelperStaticClass>(eventPayload, cancellationToken);
        }

        public Task PublishAsync(TestEventFromHelperClass eventPayload, CancellationToken cancellationToken)
        {
            return PublishAsync<TestEventFromHelperClass>(eventPayload, cancellationToken);
        }

        public Task PublishAsync(TestEventFromLocalStaticHelper eventPayload, CancellationToken cancellationToken)
        {
            return PublishAsync<TestEventFromLocalStaticHelper>(eventPayload, cancellationToken);
        }

        public Task PublishAsync(TestEventFromLocalHelper eventPayload, CancellationToken cancellationToken)
        {
            return PublishAsync<TestEventFromLocalHelper>(eventPayload, cancellationToken);
        }
    }
}
