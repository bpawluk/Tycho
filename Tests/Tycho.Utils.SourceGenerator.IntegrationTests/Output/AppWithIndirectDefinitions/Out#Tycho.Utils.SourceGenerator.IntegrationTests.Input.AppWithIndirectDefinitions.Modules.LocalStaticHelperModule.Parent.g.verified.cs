//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithIndirectDefinitions.Modules.LocalStaticHelperModule.Parent.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Structure.Parent;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithIndirectDefinitions.Modules
{
    internal class LocalStaticHelperModuleParent : ParentBase, LocalStaticHelperModule.IParent
    {
        public LocalStaticHelperModuleParent(IParentReference parentReference) : base(parentReference) { }
    }
}
