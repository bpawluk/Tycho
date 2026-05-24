//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppInGenericOuterTypes.Outer`1.Inner`1.TestApp`1.Facade.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Apps.Instance;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppInGenericOuterTypes
{
    public partial class Outer<TOuter> where TOuter : class
    {
        public partial class Inner<TInner> where TInner : notnull
        {
            internal class TestAppFacade<TApp> : AppFacadeBase, ITestApp<TApp>
                where TApp : new()
            {
                public TestAppFacade(IApp app) : base(app) { }
            }
        }
    }
}
