//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithIndirectDefinitions.TestApp.Publisher.Interface.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Apps;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithIndirectDefinitions
{
    public partial class TestApp : TychoApp
    {
        public interface IPublisher
        {
            Task PublishAsync(TestEventFromHelperExtension eventPayload, CancellationToken cancellationToken = default);

            Task PublishAsync(TestEventFromHelperStaticClass eventPayload, CancellationToken cancellationToken = default);

            Task PublishAsync(TestEventFromHelperClass eventPayload, CancellationToken cancellationToken = default);

            Task PublishAsync(TestEventFromLocalStaticHelper eventPayload, CancellationToken cancellationToken = default);

            Task PublishAsync(TestEventFromLocalHelper eventPayload, CancellationToken cancellationToken = default);
        }
    }
}
