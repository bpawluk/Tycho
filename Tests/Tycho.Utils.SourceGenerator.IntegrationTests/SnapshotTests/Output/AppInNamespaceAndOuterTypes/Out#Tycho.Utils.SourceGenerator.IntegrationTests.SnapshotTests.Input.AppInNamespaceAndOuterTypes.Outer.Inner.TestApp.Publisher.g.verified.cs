//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppInNamespaceAndOuterTypes.Outer.Inner.TestApp.Publisher.g.cs
namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppInNamespaceAndOuterTypes
{
    public partial class Outer
    {
        public partial class Inner
        {
            internal class TestAppPublisher : global::Tycho.Events.Publishing.PublisherBase, ITestAppPublisher
            {
                public TestAppPublisher(global::Tycho.Events.Publishing.IEventPublisher genericPublisher) : base(genericPublisher) { }
            }
        }
    }
}
