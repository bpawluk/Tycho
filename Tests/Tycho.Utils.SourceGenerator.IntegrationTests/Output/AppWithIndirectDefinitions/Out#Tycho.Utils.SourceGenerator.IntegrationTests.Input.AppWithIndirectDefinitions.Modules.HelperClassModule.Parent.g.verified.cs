//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithIndirectDefinitions.Modules.HelperClassModule.Parent.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Structure.Parent;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithIndirectDefinitions.Modules
{
    internal class HelperClassModuleParent : ParentBase, HelperClassModule.IParent
    {
        public HelperClassModuleParent(IParentReference parentReference) : base(parentReference) { }
    }
}
