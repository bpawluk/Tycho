//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithDownstreamContract.TestApp.Publisher.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Publishing;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithDownstreamContract
{
    internal class TestAppPublisher : PublisherBase, ITestApp.IPublisher
    {
        public TestAppPublisher(IEventPublisher genericPublisher) : base(genericPublisher) { }
    }
}
