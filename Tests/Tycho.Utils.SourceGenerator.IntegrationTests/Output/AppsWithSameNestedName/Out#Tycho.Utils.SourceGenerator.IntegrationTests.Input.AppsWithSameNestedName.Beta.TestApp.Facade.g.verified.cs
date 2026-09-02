//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppsWithSameNestedName.Beta.TestApp.Facade.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Apps.Instance;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppsWithSameNestedName
{
    public partial class Beta
    {
        internal class TestAppFacade : AppFacadeBase, ITestApp
        {
            public TestAppFacade(IApp app) : base(app) { }
        }
    }
}
