using Microsoft.Extensions.DependencyInjection;
using Tycho.Apps;
using Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithConstrainedGenericDefinition.Model;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithConstrainedGenericDefinition;

public interface IMarker { }

[TychoDefinition]
public class TestApp<TPayload, TKey> : TychoApp
    where TPayload : PayloadBase, IMarker, new()
    where TKey : notnull
{
    protected override void DefineContract(IAppContract app) { }
    protected override void DefineEvents(IAppEvents app) { }
    protected override void IncludeModules(IAppStructure app) { }
    protected override void RegisterServices(IServiceCollection app) { }
}
