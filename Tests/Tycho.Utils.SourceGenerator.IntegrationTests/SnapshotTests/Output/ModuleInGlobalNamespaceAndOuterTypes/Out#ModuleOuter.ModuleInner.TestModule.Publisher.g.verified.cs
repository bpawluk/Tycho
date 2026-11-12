//HintName: ModuleOuter.ModuleInner.TestModule.Publisher.g.cs
public partial class ModuleOuter
{
    public partial class ModuleInner
    {
        internal class TestModulePublisher : global::Tycho.Events.Publishing.PublisherBase, ITestModulePublisher
        {
            public TestModulePublisher(global::Tycho.Events.Publishing.IEventPublisher genericPublisher) : base(genericPublisher) { }
        }
    }
}
