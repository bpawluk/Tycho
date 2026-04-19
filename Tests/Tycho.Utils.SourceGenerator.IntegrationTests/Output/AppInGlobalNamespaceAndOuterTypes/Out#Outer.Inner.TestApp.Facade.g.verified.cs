//HintName: Outer.Inner.TestApp.Facade.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Apps.Instance;

public partial class Outer
{
    public partial class Inner
    {
        internal class TestAppFacade : AppFacadeBase, ITestApp
        {
            public TestAppFacade(IApp app) : base(app) { }
        }
    }
}
