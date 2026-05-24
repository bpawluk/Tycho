//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithConstrainedGenericDefinition.TestApp`2.Facade.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Apps.Instance;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithConstrainedGenericDefinition
{
    internal class TestAppFacade<TPayload, TKey> : AppFacadeBase, ITestApp<TPayload, TKey>
        where TPayload : PayloadBase, IMarker, new()
        where TKey : notnull
    {
        public TestAppFacade(IApp app) : base(app) { }
    }
}
