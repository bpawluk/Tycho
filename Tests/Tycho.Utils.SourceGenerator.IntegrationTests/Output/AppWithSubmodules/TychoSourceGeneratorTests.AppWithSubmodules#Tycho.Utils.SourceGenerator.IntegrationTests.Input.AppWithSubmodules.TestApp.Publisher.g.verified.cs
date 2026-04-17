//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithSubmodules.TestApp.Publisher.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Publishing;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithSubmodules
{
    internal class TestAppPublisher : PublisherBase, TestApp.IPublisher
    {
        public TestAppPublisher(IEventPublisher genericPublisher) : base(genericPublisher) { }
    }
}
