//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions.Modules.LocalStaticHelperModule.Publisher.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Publishing;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions.Modules
{
    internal class LocalStaticHelperModulePublisher : PublisherBase, ILocalStaticHelperModulePublisher
    {
        public LocalStaticHelperModulePublisher(IEventPublisher genericPublisher) : base(genericPublisher) { }
    }
}
