//HintName: ModuleOuter.ModuleInner.TestModule.Publisher.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Publishing;

public partial class ModuleOuter
{
    public partial class ModuleInner
    {
        internal class TestModulePublisher : PublisherBase, ITestModulePublisher
        {
            public TestModulePublisher(IEventPublisher genericPublisher) : base(genericPublisher) { }
        }
    }
}
