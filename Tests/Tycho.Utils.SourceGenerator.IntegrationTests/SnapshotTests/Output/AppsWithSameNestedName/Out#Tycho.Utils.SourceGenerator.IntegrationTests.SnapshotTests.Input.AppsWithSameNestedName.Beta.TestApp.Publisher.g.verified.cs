//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppsWithSameNestedName.Beta.TestApp.Publisher.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Publishing;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppsWithSameNestedName
{
    public partial class Beta
    {
        internal class TestAppPublisher : PublisherBase, ITestAppPublisher
        {
            public TestAppPublisher(IEventPublisher genericPublisher) : base(genericPublisher) { }
        }
    }
}
