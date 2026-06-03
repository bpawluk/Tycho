//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppInNamespace.TestApp.Publisher.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Publishing;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppInNamespace
{
    internal class TestAppPublisher : PublisherBase, IITestApp.IPublisher
    {
        public TestAppPublisher(IEventPublisher genericPublisher) : base(genericPublisher) { }
    }
}
