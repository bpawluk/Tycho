//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithConstrainedGenericDefinition.TestApp`2.Facade.Interface.g.cs
using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Utils.SourceGenerator.IntegrationTests.Input.SharedConstraints;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithConstrainedGenericDefinition
{
    public interface ITestApp<TPayload, TKey> : IAsyncDisposable
        where TPayload : PayloadBase, IMarker, new()
        where TKey : notnull
    {
    }
}
