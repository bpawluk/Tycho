//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleInGenericOuterTypes.Outer`1.Inner`1.TestModule`1.Publisher.g.cs
namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleInGenericOuterTypes
{
    public partial class Outer<TOuter>
        where TOuter : class
    {
        public partial class Inner<TInner>
            where TInner : notnull
        {
            internal class TestModulePublisher<TModule> : global::Tycho.Events.Publishing.PublisherBase, ITestModulePublisher<TModule>
                where TModule : notnull
            {
                public TestModulePublisher(global::Tycho.Events.Publishing.IEventPublisher genericPublisher) : base(genericPublisher) { }
            }
        }
    }
}
