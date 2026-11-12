//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithGenericDefinition.TestModule`1.Publisher.g.cs
namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithGenericDefinition
{
    internal class TestModulePublisher<T> : global::Tycho.Events.Publishing.PublisherBase, ITestModulePublisher<T>
    {
        public TestModulePublisher(global::Tycho.Events.Publishing.IEventPublisher genericPublisher) : base(genericPublisher) { }
    }
}
