//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithGenericDefinition.TestApp`1.Publisher.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Publishing;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithGenericDefinition
{
    internal class TestAppPublisher<T> : PublisherBase, ITestApp<T>.IPublisher
    {
        public TestAppPublisher(IEventPublisher genericPublisher) : base(genericPublisher) { }
    }
}
