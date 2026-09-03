//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppInGenericOuterTypes.Outer`1.Inner`1.TestApp`1.Builder.g.cs
#nullable enable annotations

using System;
using Tycho.Apps;
using Tycho.Apps.Instance;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppInGenericOuterTypes
{
    public partial class Outer<TOuter>
        where TOuter : class
    {
        public partial class Inner<TInner>
            where TInner : notnull
        {
            public class TestAppBuilder<TApp>
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
    }
}
