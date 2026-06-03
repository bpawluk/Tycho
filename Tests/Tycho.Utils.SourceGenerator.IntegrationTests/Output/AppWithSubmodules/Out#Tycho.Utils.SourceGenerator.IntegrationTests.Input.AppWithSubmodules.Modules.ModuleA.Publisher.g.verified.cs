//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithSubmodules.Modules.ModuleA.Publisher.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Publishing;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithSubmodules.Modules
{
    internal class ModuleAPublisher : PublisherBase, IModuleA.IPublisher
    {
        public ModuleAPublisher(IEventPublisher genericPublisher) : base(genericPublisher) { }
    }
}
