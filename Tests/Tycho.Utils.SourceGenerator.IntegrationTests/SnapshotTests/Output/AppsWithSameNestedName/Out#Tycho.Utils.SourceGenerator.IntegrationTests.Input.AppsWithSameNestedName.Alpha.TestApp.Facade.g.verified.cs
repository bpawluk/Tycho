//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppsWithSameNestedName.Alpha.TestApp.Facade.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Apps.Instance;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppsWithSameNestedName
{
    public partial class Alpha
    {
        internal class TestAppFacade : AppFacadeBase, ITestApp
        {
            public TestAppFacade(IApp app) : base(app) { }
        }
    }
}
