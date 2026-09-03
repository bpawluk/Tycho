//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppsWithSameNestedName.Alpha.TestApp.Builder.g.cs
#nullable enable annotations

using System;
using Tycho.Apps;
using Tycho.Apps.Instance;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppsWithSameNestedName
{
    public partial class Alpha
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
