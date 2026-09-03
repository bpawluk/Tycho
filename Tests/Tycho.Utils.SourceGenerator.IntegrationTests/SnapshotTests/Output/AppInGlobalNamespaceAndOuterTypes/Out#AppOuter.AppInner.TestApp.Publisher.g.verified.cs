//HintName: AppOuter.AppInner.TestApp.Publisher.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Publishing;

public partial class AppOuter
{
    public partial class AppInner
    {
        internal class TestAppPublisher : PublisherBase, ITestAppPublisher
        {
            public TestAppPublisher(IEventPublisher genericPublisher) : base(genericPublisher) { }
        }
    }
}
