//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithDownstreamContract.TestModule.Publisher.g.cs
namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithDownstreamContract
{
    internal class TestModulePublisher : global::Tycho.Events.Publishing.PublisherBase, ITestModulePublisher
    {
        public TestModulePublisher(global::Tycho.Events.Publishing.IEventPublisher genericPublisher) : base(genericPublisher) { }
    }
}
