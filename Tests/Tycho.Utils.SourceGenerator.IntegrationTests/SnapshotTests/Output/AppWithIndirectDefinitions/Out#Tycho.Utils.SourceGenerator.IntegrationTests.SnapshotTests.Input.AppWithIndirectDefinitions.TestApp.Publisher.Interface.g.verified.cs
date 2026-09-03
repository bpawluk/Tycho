//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions.TestApp.Publisher.Interface.g.cs
namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions
{
    public interface ITestAppPublisher
    {
        global::System.Threading.Tasks.Task PublishAsync(global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions.TestEventFromHelperExtension eventPayload, global::System.Threading.CancellationToken cancellationToken = default);

        global::System.Threading.Tasks.Task PublishAsync(global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions.TestEventFromHelperStaticClass eventPayload, global::System.Threading.CancellationToken cancellationToken = default);

        global::System.Threading.Tasks.Task PublishAsync(global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions.TestEventFromHelperClass eventPayload, global::System.Threading.CancellationToken cancellationToken = default);

        global::System.Threading.Tasks.Task PublishAsync(global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions.TestEventFromLocalStaticHelper eventPayload, global::System.Threading.CancellationToken cancellationToken = default);

        global::System.Threading.Tasks.Task PublishAsync(global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions.TestEventFromLocalHelper eventPayload, global::System.Threading.CancellationToken cancellationToken = default);
    }
}
