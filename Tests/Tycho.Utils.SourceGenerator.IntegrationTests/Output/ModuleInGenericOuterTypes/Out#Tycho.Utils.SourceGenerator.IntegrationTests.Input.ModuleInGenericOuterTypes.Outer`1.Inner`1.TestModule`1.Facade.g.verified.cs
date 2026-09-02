//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleInGenericOuterTypes.Outer`1.Inner`1.TestModule`1.Facade.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Modules.Instance;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleInGenericOuterTypes
{
    public partial class Outer<TOuter>
        where TOuter : class
    {
        public partial class Inner<TInner>
            where TInner : notnull
        {
            internal class TestModuleFacade<TModule> : ModuleFacadeBase, ITestModule<TModule>
                where TModule : notnull
            {
                public TestModuleFacade(IModule<TestModule<TModule>> module) : base(module) { }
            }
        }
    }
}
