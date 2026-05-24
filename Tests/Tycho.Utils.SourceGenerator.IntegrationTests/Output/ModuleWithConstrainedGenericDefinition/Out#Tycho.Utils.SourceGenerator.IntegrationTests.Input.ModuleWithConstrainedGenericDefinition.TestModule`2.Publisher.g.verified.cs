//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithConstrainedGenericDefinition.TestModule`2.Publisher.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Publishing;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithConstrainedGenericDefinition
{
    internal class TestModulePublisher<TPayload, TKey> : PublisherBase, TestModule<TPayload, TKey>.IPublisher
        where TPayload : PayloadBase, IMarker, new()
        where TKey : notnull
    {
        public TestModulePublisher(IEventPublisher genericPublisher) : base(genericPublisher) { }
    }
}
