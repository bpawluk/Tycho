//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithConstrainedGenericDefinition.TestModule`2.Publisher.Interface.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Modules;
using Tycho.Utils.SourceGenerator.IntegrationTests.Input.SharedConstraints;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithConstrainedGenericDefinition
{
    public partial class TestModule<TPayload, TKey> : TychoModule
        where TPayload : PayloadBase, IMarker, new()
        where TKey : notnull
    {
        public interface IPublisher
        {
        }
    }
}
