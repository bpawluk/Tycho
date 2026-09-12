//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithSubmodules.Modules.Outer`1.Inner.ModuleA.Facade.g.cs
namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithSubmodules.Modules
{
    public partial class Outer<TOuter>
    {
        public partial class Inner
        {
            internal class ModuleAFacade : global::Tycho.Modules.Instance.ModuleFacadeBase, IModuleA
            {
                public ModuleAFacade(global::Tycho.Modules.Instance.IModule<ModuleA> module) : base(module) { }
            }
        }
    }
}
