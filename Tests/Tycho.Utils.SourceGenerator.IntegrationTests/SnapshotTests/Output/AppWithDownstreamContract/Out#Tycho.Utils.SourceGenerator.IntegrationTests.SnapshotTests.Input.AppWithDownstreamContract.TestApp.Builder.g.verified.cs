//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithDownstreamContract.TestApp.Builder.g.cs
#nullable enable annotations

using System;
using Tycho.Apps;
using Tycho.Apps.Instance;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithDownstreamContract
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
