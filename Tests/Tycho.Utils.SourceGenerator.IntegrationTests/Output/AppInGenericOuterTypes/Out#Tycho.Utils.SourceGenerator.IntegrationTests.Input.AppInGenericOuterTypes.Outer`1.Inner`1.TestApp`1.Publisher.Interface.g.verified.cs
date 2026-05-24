//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppInGenericOuterTypes.Outer`1.Inner`1.TestApp`1.Publisher.Interface.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Apps;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppInGenericOuterTypes
{
    public partial class Outer<TOuter> where TOuter : class
    {
        public partial class Inner<TInner> where TInner : notnull
        {
            public partial class TestApp<TApp> : TychoApp
                where TApp : new()
            {
                public interface IPublisher
                {
                }
            }
        }
    }
}
