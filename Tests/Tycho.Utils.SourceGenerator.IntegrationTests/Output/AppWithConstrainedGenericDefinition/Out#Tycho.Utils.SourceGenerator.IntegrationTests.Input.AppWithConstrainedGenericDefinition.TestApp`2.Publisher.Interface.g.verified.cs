//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithConstrainedGenericDefinition.TestApp`2.Publisher.Interface.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Utils.SourceGenerator.IntegrationTests.Input.SharedConstraints;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithConstrainedGenericDefinition
{
    public partial interface ITestApp<TPayload, TKey>
        where TPayload : PayloadBase, IMarker, new()
        where TKey : notnull
    {
        public interface IPublisher
        {
        }
    }
}
