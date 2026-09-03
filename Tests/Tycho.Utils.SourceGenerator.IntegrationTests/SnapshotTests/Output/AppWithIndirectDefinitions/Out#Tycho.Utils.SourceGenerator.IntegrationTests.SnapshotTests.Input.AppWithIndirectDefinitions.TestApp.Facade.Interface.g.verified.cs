//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions.TestApp.Facade.Interface.g.cs
using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Structure;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions
{
    public interface ITestApp : IRunnable, IDisposable
    {
        Task<String> ExecuteAsync(TestRequestFromHelperExtension requestData, CancellationToken cancellationToken = default);

        Task<String> ExecuteAsync(TestRequestFromHelperStaticClass requestData, CancellationToken cancellationToken = default);

        Task<String> ExecuteAsync(TestRequestFromHelperClass requestData, CancellationToken cancellationToken = default);

        Task<String> ExecuteAsync(TestRequestFromLocalStaticHelper requestData, CancellationToken cancellationToken = default);

        Task<String> ExecuteAsync(TestRequestFromLocalHelper requestData, CancellationToken cancellationToken = default);
    }
}
