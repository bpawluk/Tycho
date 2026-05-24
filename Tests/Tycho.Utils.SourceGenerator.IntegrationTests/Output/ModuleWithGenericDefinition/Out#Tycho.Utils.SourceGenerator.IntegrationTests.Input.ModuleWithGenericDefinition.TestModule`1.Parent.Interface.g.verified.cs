//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithGenericDefinition.TestModule`1.Parent.Interface.g.cs
using System.Threading;
using System.Threading.Tasks;
using Tycho.Modules;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithGenericDefinition
{
    public partial class TestModule<T> : TychoModule
    {
        public interface IParent
        {
        }
    }
}
