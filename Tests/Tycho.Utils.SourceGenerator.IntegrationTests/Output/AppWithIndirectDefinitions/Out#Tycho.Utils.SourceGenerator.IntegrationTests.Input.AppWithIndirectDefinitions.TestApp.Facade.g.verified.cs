//HintName: Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithIndirectDefinitions.TestApp.Facade.g.cs
using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Apps.Instance;
using Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithIndirectDefinitions;

namespace Tycho.Utils.SourceGenerator.IntegrationTests.Input.AppWithIndirectDefinitions
{
    internal class TestAppFacade : AppFacadeBase, ITestApp
    {
        public TestAppFacade(IApp app) : base(app) { }

        public Task<String> ExecuteAsync(TestRequestFromLocalHelper requestData, CancellationToken cancellationToken)
        {
            return ExecuteAsync<TestRequestFromLocalHelper, String>(requestData, cancellationToken);
        }    

        public Task<String> ExecuteAsync(TestRequestFromLocalStaticHelper requestData, CancellationToken cancellationToken)
        {
            return ExecuteAsync<TestRequestFromLocalStaticHelper, String>(requestData, cancellationToken);
        }    

        public Task<String> ExecuteAsync(TestRequestFromHelperClass requestData, CancellationToken cancellationToken)
        {
            return ExecuteAsync<TestRequestFromHelperClass, String>(requestData, cancellationToken);
        }    

        public Task<String> ExecuteAsync(TestRequestFromHelperStaticClass requestData, CancellationToken cancellationToken)
        {
            return ExecuteAsync<TestRequestFromHelperStaticClass, String>(requestData, cancellationToken);
        }    

        public Task<String> ExecuteAsync(TestRequestFromHelperExtension requestData, CancellationToken cancellationToken)
        {
            return ExecuteAsync<TestRequestFromHelperExtension, String>(requestData, cancellationToken);
        }    
    }
}
