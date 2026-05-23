//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithIndirectDefinitions.Modules.LocalHelperModule.Parent.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Structure.Parent;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithIndirectDefinitions.Modules
{
    internal class LocalHelperModuleParent : ParentBase, LocalHelperModule.IParent
    {
        public LocalHelperModuleParent(IParentReference parentReference) : base(parentReference) { }
    }
}
