using Microsoft.Extensions.DependencyInjection;
using Tycho.Apps;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithConstrainedGenericDefinition
{
    public abstract class PayloadBase { }

    public interface IMarker { }

    [TychoDefinition]
    public partial class TestApp<TPayload, TKey> : TychoApp
        where TPayload : PayloadBase, IMarker, new()
        where TKey : notnull
    {
        protected override void DefineContract(IAppContract app) { }
        protected override void DefineEvents(IAppEvents app) { }
        protected override void IncludeModules(IAppStructure app) { }
        protected override void RegisterServices(IServiceCollection app) { }
    }
}
