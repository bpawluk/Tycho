//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithGenericDefinition.TestModule`1.Publisher.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Publishing;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithGenericDefinition
{
    internal class TestModulePublisher<T> : PublisherBase, TestModule<T>.IPublisher
    {
        public TestModulePublisher(IEventPublisher genericPublisher) : base(genericPublisher) { }
    }
}
