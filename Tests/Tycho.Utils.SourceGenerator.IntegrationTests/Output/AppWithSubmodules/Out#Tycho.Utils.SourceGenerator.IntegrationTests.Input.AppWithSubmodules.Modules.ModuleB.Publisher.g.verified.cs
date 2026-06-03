//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithSubmodules.Modules.ModuleB.Publisher.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Publishing;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithSubmodules.Modules
{
    internal class ModuleBPublisher : PublisherBase, IModuleBPublisher
    {
        public ModuleBPublisher(IEventPublisher genericPublisher) : base(genericPublisher) { }
    }
}
