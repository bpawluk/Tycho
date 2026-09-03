//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions.Modules.LocalHelperModule.Publisher.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Publishing;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithIndirectDefinitions.Modules
{
    internal class LocalHelperModulePublisher : PublisherBase, ILocalHelperModulePublisher
    {
        public LocalHelperModulePublisher(IEventPublisher genericPublisher) : base(genericPublisher) { }
    }
}
