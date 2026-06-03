//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleInNamespace.TestModule.Publisher.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Publishing;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleInNamespace
{
    internal class TestModulePublisher : PublisherBase, ITestModulePublisher
    {
        public TestModulePublisher(IEventPublisher genericPublisher) : base(genericPublisher) { }
    }
}
