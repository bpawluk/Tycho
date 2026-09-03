//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithGenericDefinition.TestModule`1.Publisher.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Publishing;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.ModuleWithGenericDefinition
{
    internal class TestModulePublisher<T> : PublisherBase, ITestModulePublisher<T>
    {
        public TestModulePublisher(IEventPublisher genericPublisher) : base(genericPublisher) { }
    }
}
