//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithConstrainedGenericDefinition.TestApp`2.Builder.g.cs
#nullable enable annotations

using System;
using Tycho.Apps;
using Tycho.Apps.Instance;
using Tycho.Utils.SourceGenerator.IntegrationTests.Input.SharedConstraints;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithConstrainedGenericDefinition
{
    public class TestAppBuilder<TPayload, TKey>
        where TPayload : PayloadBase, IMarker, new()
        where TKey : notnull
    {
        private readonly IAppBuilderBase _appBuilderBase;

        public TestAppBuilder(IAppBuilderBase appBuilderBase)
        {
            _appBuilderBase = appBuilderBase;
        }

        public ITestApp<TPayload, TKey> Build(IServiceProvider? parentServiceProvider = null)
        {
            IApp app = _appBuilderBase.Build(parentServiceProvider);
            return new TestAppFacade<TPayload, TKey>(app);
        }
    }
}
