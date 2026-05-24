using Microsoft.Extensions.DependencyInjection;
using Tycho.Apps;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithGenericDefinition
{
    [TychoDefinition]
    public partial class TestApp<T> : TychoApp
    {
        protected override void DefineContract(IAppContract app) { }
        protected override void DefineEvents(IAppEvents app) { }
        protected override void IncludeModules(IAppStructure app) { }
        protected override void RegisterServices(IServiceCollection app) { }
    }
}
