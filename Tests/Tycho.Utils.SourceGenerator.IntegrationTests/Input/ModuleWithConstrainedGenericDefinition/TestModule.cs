using Microsoft.Extensions.DependencyInjection;
using Tycho.Modules;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithConstrainedGenericDefinition
{
    public abstract class PayloadBase { }

    public interface IMarker { }

    [TychoDefinition]
    public partial class TestModule<TPayload, TKey> : TychoModule
        where TPayload : PayloadBase, IMarker, new()
        where TKey : notnull
    {
        protected override void DefineContract(IModuleContract module) { }
        protected override void DefineEvents(IModuleEvents module) { }
        protected override void IncludeModules(IModuleStructure module) { }
        protected override void RegisterServices(IServiceCollection module) { }
    }
}
