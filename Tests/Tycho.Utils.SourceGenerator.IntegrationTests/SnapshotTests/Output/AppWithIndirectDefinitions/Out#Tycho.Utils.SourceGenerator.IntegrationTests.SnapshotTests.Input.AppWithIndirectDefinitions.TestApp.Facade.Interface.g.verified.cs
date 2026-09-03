//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions.TestApp.Facade.Interface.g.cs
namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions
{
    public interface ITestApp : global::Tycho.Structure.IRunnable, global::System.IDisposable
    {
        global::System.Threading.Tasks.Task<global::System.String> ExecuteAsync(global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions.TestRequestFromHelperExtension requestData, global::System.Threading.CancellationToken cancellationToken = default);

        global::System.Threading.Tasks.Task<global::System.String> ExecuteAsync(global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions.TestRequestFromHelperStaticClass requestData, global::System.Threading.CancellationToken cancellationToken = default);

        global::System.Threading.Tasks.Task<global::System.String> ExecuteAsync(global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions.TestRequestFromHelperClass requestData, global::System.Threading.CancellationToken cancellationToken = default);

        global::System.Threading.Tasks.Task<global::System.String> ExecuteAsync(global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions.TestRequestFromLocalStaticHelper requestData, global::System.Threading.CancellationToken cancellationToken = default);

        global::System.Threading.Tasks.Task<global::System.String> ExecuteAsync(global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions.TestRequestFromLocalHelper requestData, global::System.Threading.CancellationToken cancellationToken = default);
    }
}
