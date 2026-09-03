//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithConstrainedGenericDefinition.TestModule`2.Facade.Interface.g.cs
using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Structure;
using Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithConstrainedGenericDefinition.Model;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithConstrainedGenericDefinition
{
    public interface ITestModule<TPayload, TKey> : IRunnable, IDisposable
        where TPayload : PayloadBase, IMarker, new()
        where TKey : notnull
    {
    }
}
