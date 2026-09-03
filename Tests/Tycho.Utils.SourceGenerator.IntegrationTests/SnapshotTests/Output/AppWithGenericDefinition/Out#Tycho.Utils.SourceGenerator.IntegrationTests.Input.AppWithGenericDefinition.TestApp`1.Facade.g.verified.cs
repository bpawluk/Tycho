//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithGenericDefinition.TestApp`1.Facade.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Apps.Instance;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithGenericDefinition
{
    internal class TestAppFacade<T> : AppFacadeBase, ITestApp<T>
    {
        public TestAppFacade(IApp app) : base(app) { }
    }
}
