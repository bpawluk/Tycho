//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithGenericDefinition.TestApp`1.Facade.g.cs
namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppWithGenericDefinition
{
    internal class TestAppFacade<T> : global::Tycho.Apps.Instance.AppFacadeBase, ITestApp<T>
    {
        public TestAppFacade(global::Tycho.Apps.Instance.IApp app) : base(app) { }
    }
}
