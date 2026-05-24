//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleInGenericOuterTypes.Outer`1.Inner`1.TestModule`1.Publisher.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Publishing;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleInGenericOuterTypes
{
    public partial class Outer<TOuter> where TOuter : class
    {
        public partial class Inner<TInner> where TInner : notnull
        {
            internal class TestModulePublisher<TModule> : PublisherBase, TestModule<TModule>.IPublisher
                where TModule : notnull
            {
                public TestModulePublisher(IEventPublisher genericPublisher) : base(genericPublisher) { }
            }
        }
    }
}
