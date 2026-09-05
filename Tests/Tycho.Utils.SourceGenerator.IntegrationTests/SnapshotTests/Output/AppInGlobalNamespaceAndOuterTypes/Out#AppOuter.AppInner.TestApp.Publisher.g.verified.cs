//HintName: AppOuter.AppInner.TestApp.Publisher.g.cs
public partial class AppOuter
{
    public partial class AppInner
    {
        internal class TestAppPublisher : global::Tycho.Events.Publishing.PublisherBase, ITestAppPublisher
        {
            public TestAppPublisher(global::Tycho.Events.Publishing.IEventPublisher genericPublisher) : base(genericPublisher) { }
        }
    }
}
