//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppInGenericOuterTypes.Outer`1.Inner`1.TestApp`1.Facade.Interface.g.cs
using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Structure;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.SnapshotTests.Input.AppInGenericOuterTypes
{
    public partial class Outer<TOuter>
        where TOuter : class
    {
        public partial class Inner<TInner>
            where TInner : notnull
        {
            public interface ITestApp<TApp> : IRunnable, IDisposable
                where TApp : new()
            {
            }
        }
    }
}
