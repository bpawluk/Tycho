//HintName: TestApp.Publisher.g.cs
internal class TestAppPublisher : global::Tycho.Events.Publishing.PublisherBase, ITestAppPublisher
{
    public TestAppPublisher(global::Tycho.Events.Publishing.IEventPublisher genericPublisher) : base(genericPublisher) { }
}
