//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppInGenericOuterTypes.Outer`1.Inner`1.TestApp`1.Facade.Interface.g.cs
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppInGenericOuterTypes
{
    public partial class Outer<TOuter>
        where TOuter : class
    {
        public partial class Inner<TInner>
            where TInner : notnull
        {
            public partial interface ITestApp<TApp> : IAsyncDisposable
                where TApp : new()
            {
            }
        }
    }
}
