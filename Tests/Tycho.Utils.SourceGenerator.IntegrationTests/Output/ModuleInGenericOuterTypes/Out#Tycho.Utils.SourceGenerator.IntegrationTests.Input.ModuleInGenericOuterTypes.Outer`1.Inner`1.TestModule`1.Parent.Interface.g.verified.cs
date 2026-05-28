//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleInGenericOuterTypes.Outer`1.Inner`1.TestModule`1.Parent.Interface.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Modules;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleInGenericOuterTypes
{
    public partial class Outer<TOuter>
        where TOuter : class
    {
        public partial class Inner<TInner>
            where TInner : notnull
        {
            public partial class TestModule<TModule> : TychoModule
                where TModule : notnull
            {
                public interface IParent
                {
                }
            }
        }
    }
}
