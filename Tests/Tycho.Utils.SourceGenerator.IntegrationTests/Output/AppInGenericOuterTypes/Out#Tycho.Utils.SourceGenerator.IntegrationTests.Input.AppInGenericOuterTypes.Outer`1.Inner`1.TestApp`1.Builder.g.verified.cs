//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppInGenericOuterTypes.Outer`1.Inner`1.TestApp`1.Builder.g.cs
#nullable enable annotations

using System;
using Tycho.Apps;
using Tycho.Apps.Instance;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppInGenericOuterTypes
{
    public class TestAppBuilder<TOuter, TInner, TApp>
        where TOuter : class
        where TInner : notnull
        where TApp : new()
    {
        private readonly IAppBuilderBase _appBuilderBase;

        public TestAppBuilder(IAppBuilderBase appBuilderBase)
        {
            _appBuilderBase = appBuilderBase;
        }

        public ITestApp<TApp> Build(IServiceProvider? parentServiceProvider = null)
        {
            IApp app = _appBuilderBase.Build(parentServiceProvider);
            return new TestAppFacade<TApp>(app);
        }
    }
}
