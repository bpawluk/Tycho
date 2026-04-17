//HintName: Outer.Inner.TestApp.Publisher.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Publishing;

    public partial class Outer
    {
    public partial class Inner
    {
    internal class TestAppPublisher : PublisherBase, TestApp.IPublisher
    {
        public TestAppPublisher(IEventPublisher genericPublisher) : base(genericPublisher) { }
    }
    }
    }
