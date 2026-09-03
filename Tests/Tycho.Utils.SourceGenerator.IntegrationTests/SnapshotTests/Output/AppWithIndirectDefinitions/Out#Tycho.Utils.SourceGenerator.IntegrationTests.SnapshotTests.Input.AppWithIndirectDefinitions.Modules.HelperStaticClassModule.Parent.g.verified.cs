//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions.Modules.HelperStaticClassModule.Parent.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Structure.Parent;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions.Modules
{
    internal class HelperStaticClassModuleParent : ParentBase, IHelperStaticClassModuleParent
    {
        public HelperStaticClassModuleParent(IParentReference parentReference) : base(parentReference) { }
    }
}
