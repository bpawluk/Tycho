using Microsoft.Extensions.DependencyInjection;
using Tycho.Apps;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppInGenericOuterTypes
{
    public partial class Outer<TOuter>
        where TOuter : class
    {
        public partial class Inner<TInner>
            where TInner : notnull
        {
            [TychoDefinition]
            public class TestApp<TApp> : TychoApp
                where TApp : new()
            {
                protected override void DefineContract(IAppContract app) { }
                protected override void DefineEvents(IAppEvents app) { }
                protected override void IncludeModules(IAppStructure app) { }
                protected override void RegisterServices(IServiceCollection app) { }
            }
        }
    }
}
