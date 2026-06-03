//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithConstrainedGenericDefinition.TestApp`2.Publisher.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Publishing;
using Tycho.Utils.SourceGenerator.IntegrationTests.Input.SharedConstraints;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithConstrainedGenericDefinition
{
    internal class TestAppPublisher<TPayload, TKey> : PublisherBase, ITestApp<TPayload, TKey>.IPublisher
        where TPayload : PayloadBase, IMarker, new()
        where TKey : notnull
    {
        public TestAppPublisher(IEventPublisher genericPublisher) : base(genericPublisher) { }
    }
}
