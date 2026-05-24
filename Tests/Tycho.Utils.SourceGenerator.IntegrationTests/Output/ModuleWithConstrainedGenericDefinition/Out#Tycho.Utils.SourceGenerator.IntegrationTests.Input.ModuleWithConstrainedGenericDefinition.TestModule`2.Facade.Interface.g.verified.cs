//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithConstrainedGenericDefinition.TestModule`2.Facade.Interface.g.cs
using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithConstrainedGenericDefinition
{
    public interface ITestModule<TPayload, TKey>
        where TPayload : PayloadBase, IMarker, new()
        where TKey : notnull
    {
    }
}
