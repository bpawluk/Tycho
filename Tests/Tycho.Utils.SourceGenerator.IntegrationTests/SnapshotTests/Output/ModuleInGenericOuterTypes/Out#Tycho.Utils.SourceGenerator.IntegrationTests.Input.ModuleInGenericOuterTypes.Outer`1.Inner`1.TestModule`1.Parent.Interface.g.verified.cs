//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleInGenericOuterTypes.Outer`1.Inner`1.TestModule`1.Parent.Interface.g.cs
using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleInGenericOuterTypes
{
    public partial class Outer<TOuter>
        where TOuter : class
    {
        public partial class Inner<TInner>
            where TInner : notnull
        {
            public interface ITestModuleParent<TModule>
                where TModule : notnull
            {
            }
        }
    }
}
