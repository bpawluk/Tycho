//HintName: AppOuter.AppInner.TestApp.Facade.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Apps.Instance;

public partial class AppOuter
{
    public partial class AppInner
    {
        internal class TestAppFacade : AppFacadeBase, ITestApp
        {
            public TestAppFacade(IApp app) : base(app) { }
        }
    }
}
