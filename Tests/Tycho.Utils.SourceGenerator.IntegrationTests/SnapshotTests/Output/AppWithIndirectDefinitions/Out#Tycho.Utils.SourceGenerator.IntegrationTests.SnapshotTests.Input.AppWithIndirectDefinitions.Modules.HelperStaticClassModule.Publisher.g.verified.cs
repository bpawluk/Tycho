//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions.Modules.HelperStaticClassModule.Publisher.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Publishing;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions.Modules
{
    internal class HelperStaticClassModulePublisher : PublisherBase, IHelperStaticClassModulePublisher
    {
        public HelperStaticClassModulePublisher(IEventPublisher genericPublisher) : base(genericPublisher) { }
    }
}
