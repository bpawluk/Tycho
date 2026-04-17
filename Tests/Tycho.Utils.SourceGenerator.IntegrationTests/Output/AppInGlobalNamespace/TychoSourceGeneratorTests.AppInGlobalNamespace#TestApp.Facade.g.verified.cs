//HintName: TestApp.Facade.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Apps.Instance;

    internal class TestAppFacade : AppFacadeBase, ITestApp
    {
        public TestAppFacade(IApp app) : base(app) { }
    }
