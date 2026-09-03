//HintName: Outer.Inner.TestApp.Builder.g.cs
#nullable enable annotations

using System;
using Tycho.Apps;
using Tycho.Apps.Instance;

public partial class Outer
{
    public partial class Inner
    {
        public class TestAppBuilder
        {
            private readonly IAppBuilderBase _appBuilderBase;

            public TestAppBuilder(IAppBuilderBase appBuilderBase)
            {
                _appBuilderBase = appBuilderBase;
            }

            public ITestApp Build(IServiceProvider? parentServiceProvider = null)
            {
                IApp app = _appBuilderBase.Build(parentServiceProvider);
                return new TestAppFacade(app);
            }
        }
    }
}
