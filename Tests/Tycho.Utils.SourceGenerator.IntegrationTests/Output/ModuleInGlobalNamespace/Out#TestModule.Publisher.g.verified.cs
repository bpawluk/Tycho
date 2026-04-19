//HintName: TestModule.Publisher.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Publishing;

internal class TestModulePublisher : PublisherBase, TestModule.IPublisher
{
    public TestModulePublisher(IEventPublisher genericPublisher) : base(genericPublisher) { }
}
