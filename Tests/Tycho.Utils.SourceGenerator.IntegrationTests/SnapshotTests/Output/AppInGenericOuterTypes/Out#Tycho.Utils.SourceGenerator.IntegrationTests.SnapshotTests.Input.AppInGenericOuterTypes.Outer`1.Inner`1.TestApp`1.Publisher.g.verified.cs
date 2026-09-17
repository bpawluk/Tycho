//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppInGenericOuterTypes.Outer`1.Inner`1.TestApp`1.Publisher.g.cs
namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppInGenericOuterTypes
{
    public partial class Outer<TOuter>
        where TOuter : class
    {
        public partial class Inner<TInner>
            where TInner : notnull
        {
            internal class TestAppPublisher<TApp> : global::Tycho.Events.Publishing.PublisherBase, ITestAppPublisher<TApp>
                where TApp : new()
            {
                public TestAppPublisher(global::Tycho.Events.Publishing.IEventPublisher genericPublisher) : base(genericPublisher) { }
            }
        }
    }
}
