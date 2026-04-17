//HintName: TestApp.Publisher.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Publishing;

    internal class TestAppPublisher : PublisherBase, TestApp.IPublisher
    {
        public TestAppPublisher(IEventPublisher genericPublisher) : base(genericPublisher) { }
    }
