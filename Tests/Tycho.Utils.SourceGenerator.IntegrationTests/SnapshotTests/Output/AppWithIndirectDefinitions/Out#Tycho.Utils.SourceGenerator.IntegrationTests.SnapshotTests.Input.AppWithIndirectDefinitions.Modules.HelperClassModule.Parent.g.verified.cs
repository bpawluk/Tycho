//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions.Modules.HelperClassModule.Parent.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Structure.Parent;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions.Modules
{
    internal class HelperClassModuleParent : ParentBase, IHelperClassModuleParent
    {
        public HelperClassModuleParent(IParentReference parentReference) : base(parentReference) { }
    }
}
