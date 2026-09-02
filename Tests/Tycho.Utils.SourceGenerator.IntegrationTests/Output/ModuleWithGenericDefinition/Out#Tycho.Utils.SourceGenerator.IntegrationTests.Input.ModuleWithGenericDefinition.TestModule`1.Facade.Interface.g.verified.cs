//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithGenericDefinition.TestModule`1.Facade.Interface.g.cs
using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Structure;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.ModuleWithGenericDefinition
{
    public interface ITestModule<T> : IRunnable, IDisposable
    {
    }
}
