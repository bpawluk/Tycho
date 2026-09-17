//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleInNamespaceAndOuterTypes.Outer.Inner.TestModule.Publisher.g.cs
namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleInNamespaceAndOuterTypes
{
    public partial class Outer
    {
        public partial class Inner
        {
            internal class TestModulePublisher : global::Tycho.Events.Publishing.PublisherBase, ITestModulePublisher
            {
                public TestModulePublisher(global::Tycho.Events.Publishing.IEventPublisher genericPublisher) : base(genericPublisher) { }
            }
        }
    }
}
