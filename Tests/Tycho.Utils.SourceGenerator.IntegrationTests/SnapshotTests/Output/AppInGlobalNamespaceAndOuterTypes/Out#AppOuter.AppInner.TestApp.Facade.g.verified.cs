//HintName: AppOuter.AppInner.TestApp.Facade.g.cs
public partial class AppOuter
{
    public partial class AppInner
    {
        internal class TestAppFacade : global::Tycho.Apps.Instance.AppFacadeBase, ITestApp
        {
            public TestAppFacade(global::Tycho.Apps.Instance.IApp app) : base(app) { }
        }
    }
}
