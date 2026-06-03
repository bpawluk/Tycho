//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppInNamespaceAndOuterTypes.Outer.Inner.TestApp.Publisher.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Publishing;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppInNamespaceAndOuterTypes
{
    public partial class Outer
    {
        public partial class Inner
        {
            internal class TestAppPublisher : PublisherBase, IITestApp.IPublisher
            {
                public TestAppPublisher(IEventPublisher genericPublisher) : base(genericPublisher) { }
            }
        }
    }
}
