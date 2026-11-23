//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppsWithSameNestedName.Alpha.TestApp.Publisher.g.cs
namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppsWithSameNestedName
{
    public partial class Alpha
    {
        internal class TestAppPublisher : global::Tycho.Events.Publishing.PublisherBase, ITestAppPublisher
        {
            public TestAppPublisher(global::Tycho.Events.Publishing.IEventPublisher genericPublisher) : base(genericPublisher) { }
        }
    }
}
