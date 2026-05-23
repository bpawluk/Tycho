//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithIndirectDefinitions.Modules.LocalHelperModule.Publisher.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Publishing;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithIndirectDefinitions.Modules
{
    internal class LocalHelperModulePublisher : PublisherBase, LocalHelperModule.IPublisher
    {
        public LocalHelperModulePublisher(IEventPublisher genericPublisher) : base(genericPublisher) { }
    }
}
