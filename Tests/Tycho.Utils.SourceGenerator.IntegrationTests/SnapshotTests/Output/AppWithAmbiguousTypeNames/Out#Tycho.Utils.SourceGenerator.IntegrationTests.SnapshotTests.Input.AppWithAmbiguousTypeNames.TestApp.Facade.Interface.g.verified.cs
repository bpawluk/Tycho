//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithAmbiguousTypeNames.TestApp.Facade.Interface.g.cs
namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithAmbiguousTypeNames
{
    public interface ITestApp : global::Tycho.Structure.IRunnable, global::System.IDisposable
    {
        global::System.Threading.Tasks.Task ExecuteAsync(global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithAmbiguousTypeNames.Alpha.Request requestData, global::System.Threading.CancellationToken cancellationToken = default);

        global::System.Threading.Tasks.Task ExecuteAsync(global::Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithAmbiguousTypeNames.Beta.Request requestData, global::System.Threading.CancellationToken cancellationToken = default);
    }
}
