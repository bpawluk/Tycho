//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithIndirectDefinitions.Modules.HelperClassModule.Publisher.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Publishing;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithIndirectDefinitions.Modules
{
    internal class HelperClassModulePublisher : PublisherBase, IHelperClassModule.IPublisher
    {
        public HelperClassModulePublisher(IEventPublisher genericPublisher) : base(genericPublisher) { }
    }
}
