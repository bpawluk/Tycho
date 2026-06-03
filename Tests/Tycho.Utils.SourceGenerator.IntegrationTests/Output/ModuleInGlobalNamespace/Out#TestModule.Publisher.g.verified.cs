//HintName: TestModule.Publisher.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Publishing;

internal class TestModulePublisher : PublisherBase, ITestModule.IPublisher
{
    public TestModulePublisher(IEventPublisher genericPublisher) : base(genericPublisher) { }
}
