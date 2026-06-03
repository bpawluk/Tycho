//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithConstrainedGenericDefinition.TestModule`2.Parent.Interface.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Utils.SourceGenerator.IntegrationTests.Input.SharedConstraints;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithConstrainedGenericDefinition
{
    public interface ITestModuleParent<TPayload, TKey>
        where TPayload : PayloadBase, IMarker, new()
        where TKey : notnull
    {
    }
}
