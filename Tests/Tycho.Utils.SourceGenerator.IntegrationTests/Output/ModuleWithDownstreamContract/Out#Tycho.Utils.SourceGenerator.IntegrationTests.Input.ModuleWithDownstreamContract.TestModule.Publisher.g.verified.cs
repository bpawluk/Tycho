//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithDownstreamContract.TestModule.Publisher.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Publishing;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithDownstreamContract
{
    internal class TestModulePublisher : PublisherBase, TestModule.IPublisher
    {
        public TestModulePublisher(IEventPublisher genericPublisher) : base(genericPublisher) { }
    }
}
