//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleInNamespaceAndOuterTypes.Outer.Inner.TestModule.Publisher.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Publishing;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleInNamespaceAndOuterTypes
{
    public partial class Outer
    {
    public partial class Inner
    {
    internal class TestModulePublisher : PublisherBase, TestModule.IPublisher
    {
        public TestModulePublisher(IEventPublisher genericPublisher) : base(genericPublisher) { }
    }
    }
    }
}
