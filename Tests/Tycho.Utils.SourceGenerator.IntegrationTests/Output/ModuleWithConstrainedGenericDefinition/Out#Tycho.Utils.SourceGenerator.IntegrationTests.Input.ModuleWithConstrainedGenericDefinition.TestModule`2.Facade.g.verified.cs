//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithConstrainedGenericDefinition.TestModule`2.Facade.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Modules.Instance;
using Tycho.Utils.SourceGenerator.IntegrationTests.Input.SharedConstraints;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithConstrainedGenericDefinition
{
    internal class TestModuleFacade<TPayload, TKey> : ModuleFacadeBase, ITestModule<TPayload, TKey>
        where TPayload : PayloadBase, IMarker, new()
        where TKey : notnull
    {
        public TestModuleFacade(IModule<TestModule<TPayload, TKey>> module) : base(module) { }
    }
}
