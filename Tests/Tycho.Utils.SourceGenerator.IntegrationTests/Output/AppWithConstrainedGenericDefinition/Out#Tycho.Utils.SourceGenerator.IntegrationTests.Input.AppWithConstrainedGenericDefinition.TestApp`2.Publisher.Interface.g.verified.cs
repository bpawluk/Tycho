//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithConstrainedGenericDefinition.TestApp`2.Publisher.Interface.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Apps;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithConstrainedGenericDefinition
{
    public partial class TestApp<TPayload, TKey> : TychoApp
        where TPayload : PayloadBase, IMarker, new()
        where TKey : notnull
    {
        public interface IPublisher
        {
        }
    }
}
