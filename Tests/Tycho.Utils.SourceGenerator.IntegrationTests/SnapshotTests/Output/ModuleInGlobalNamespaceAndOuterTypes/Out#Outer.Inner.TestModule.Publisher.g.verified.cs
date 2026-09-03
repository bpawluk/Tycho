//HintName: Outer.Inner.TestModule.Publisher.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Publishing;

public partial class Outer
{
    public partial class Inner
    {
        internal class TestModulePublisher : PublisherBase, ITestModulePublisher
        {
            public TestModulePublisher(IEventPublisher genericPublisher) : base(genericPublisher) { }
        }
    }
}
