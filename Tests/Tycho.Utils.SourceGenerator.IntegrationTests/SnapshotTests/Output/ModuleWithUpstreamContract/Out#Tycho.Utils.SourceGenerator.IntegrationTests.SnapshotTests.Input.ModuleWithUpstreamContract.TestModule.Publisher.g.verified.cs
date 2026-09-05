//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithUpstreamContract.TestModule.Publisher.g.cs
namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithUpstreamContract
{
    internal class TestModulePublisher : global::Tycho.Events.Publishing.PublisherBase, ITestModulePublisher
    {
        public TestModulePublisher(global::Tycho.Events.Publishing.IEventPublisher genericPublisher) : base(genericPublisher) { }
    }
}
