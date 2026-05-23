//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithIndirectDefinitions.Modules.LocalStaticHelperModule.Publisher.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Publishing;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithIndirectDefinitions.Modules
{
    internal class LocalStaticHelperModulePublisher : PublisherBase, LocalStaticHelperModule.IPublisher
    {
        public LocalStaticHelperModulePublisher(IEventPublisher genericPublisher) : base(genericPublisher) { }
    }
}
