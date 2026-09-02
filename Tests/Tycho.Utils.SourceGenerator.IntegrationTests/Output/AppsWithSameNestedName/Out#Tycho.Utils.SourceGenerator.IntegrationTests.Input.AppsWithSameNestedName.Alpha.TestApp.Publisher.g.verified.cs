//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppsWithSameNestedName.Alpha.TestApp.Publisher.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Publishing;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppsWithSameNestedName
{
    public partial class Alpha
    {
        internal class TestAppPublisher : PublisherBase, ITestAppPublisher
        {
            public TestAppPublisher(IEventPublisher genericPublisher) : base(genericPublisher) { }
        }
    }
}
