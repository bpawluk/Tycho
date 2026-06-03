//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppInGenericOuterTypes.Outer`1.Inner`1.TestApp`1.Publisher.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Publishing;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppInGenericOuterTypes
{
    public partial class Outer<TOuter>
        where TOuter : class
    {
        public partial class Inner<TInner>
            where TInner : notnull
        {
            internal class TestAppPublisher<TApp> : PublisherBase, IITestApp<TApp>.IPublisher
                where TApp : new()
            {
                public TestAppPublisher(IEventPublisher genericPublisher) : base(genericPublisher) { }
            }
        }
    }
}
