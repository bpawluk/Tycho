//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithIndirectDefinitions.TestApp.Publisher.Interface.g.cs
using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithIndirectDefinitions
{
    public interface ITestAppPublisher
    {
        Task PublishAsync(TestEventFromHelperExtension eventPayload, CancellationToken cancellationToken = default);

        Task PublishAsync(TestEventFromHelperStaticClass eventPayload, CancellationToken cancellationToken = default);

        Task PublishAsync(TestEventFromHelperClass eventPayload, CancellationToken cancellationToken = default);

        Task PublishAsync(TestEventFromLocalStaticHelper eventPayload, CancellationToken cancellationToken = default);

        Task PublishAsync(TestEventFromLocalHelper eventPayload, CancellationToken cancellationToken = default);
    }
}
