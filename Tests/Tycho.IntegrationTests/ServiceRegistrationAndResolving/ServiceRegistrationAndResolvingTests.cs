using Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT;
using Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Modules;
using Tycho.IntegrationTests._Utils;

namespace Tycho.IntegrationTests.ServiceRegistrationAndResolving;

public sealed class ServiceRegistrationAndResolvingTests : IAsyncLifetime
{
    private readonly TestWorkflow<TestResult> _testWorkflow = new();
    private ITestApp _sut = null!;

    public async ValueTask InitializeAsync()
    {
        _sut = new TestApp(_testWorkflow).CreateAppBuilder().Build();
        await _sut.StartAsync(TestContext.Current.CancellationToken);
    }

    [Fact(Timeout = 5000)]
    public async Task TychoEnables_ResolvingSingletonServices_FromRequestHandlersInApps()
    {
        // Arrange
        // - no arrangement required

        // Act
        int firstResult = await _sut.ExecuteAsync(new GetAppSingletonServiceUsageRequest(), TestContext.Current.CancellationToken);
        int secondResult = await _sut.ExecuteAsync(new GetAppSingletonServiceUsageRequest(), TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(2, firstResult);
        Assert.Equal(4, secondResult);
    }

    [Fact(Timeout = 5000)]
    public async Task TychoEnables_ResolvingSingletonServices_FromRequestHandlersInModules()
    {
        // Arrange
        // - no arrangement required

        // Act
        int firstResult = await _sut.ExecuteAsync(new GetModuleSingletonServiceUsageRequest(), TestContext.Current.CancellationToken);
        int secondResult = await _sut.ExecuteAsync(new GetModuleSingletonServiceUsageRequest(), TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(2, firstResult);
        Assert.Equal(4, secondResult);
    }

    [Fact(Timeout = 5000)]
    public async Task TychoEnables_ResolvingSingletonServices_FromEventHandlersInApps()
    {
        // Arrange
        string workflowId = "event-app-singleton-workflow";
        var firstRequest = new BeginTestWorkflowRequest(new TestResult { Id = workflowId });
        var secondRequest = new BeginTestWorkflowRequest(new TestResult { Id = workflowId });

        // Act
        await _sut!.ExecuteAsync(firstRequest, TestContext.Current.CancellationToken);
        int firstResult = (await _testWorkflow.GetResult()).NumberOfCalls;
        _testWorkflow.Reset();
        await _sut!.ExecuteAsync(secondRequest, TestContext.Current.CancellationToken);
        int secondResult = (await _testWorkflow.GetResult()).NumberOfCalls;

        // Assert
        Assert.Equal(2, firstResult);
        Assert.Equal(4, secondResult);
    }

    [Fact(Timeout = 5000)]
    public async Task TychoEnables_ResolvingSingletonServices_FromEventHandlersInModules()
    {
        // Arrange
        string workflowId = "event-module-singleton-workflow";
        var firstRequest = new BeginTestWorkflowRequest(new TestResult { Id = workflowId });
        var secondRequest = new BeginTestWorkflowRequest(new TestResult { Id = workflowId });

        // Act
        await _sut!.ExecuteAsync(firstRequest, TestContext.Current.CancellationToken);
        int firstResult = (await _testWorkflow.GetResult()).NumberOfCalls;
        _testWorkflow.Reset();
        await _sut!.ExecuteAsync(secondRequest, TestContext.Current.CancellationToken);
        int secondResult = (await _testWorkflow.GetResult()).NumberOfCalls;

        // Assert
        Assert.Equal(2, firstResult);
        Assert.Equal(4, secondResult);
    }

    [Fact(Timeout = 5000)]
    public async Task TychoEnables_ResolvingScopedServices_FromRequestHandlersInApps()
    {
        // Arrange
        // - no arrangement required

        // Act
        int firstResult = await _sut.ExecuteAsync(new GetAppScopedServiceUsageRequest(), TestContext.Current.CancellationToken);
        int secondResult = await _sut.ExecuteAsync(new GetAppScopedServiceUsageRequest(), TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(2, firstResult);
        Assert.Equal(2, secondResult);
    }

    [Fact(Timeout = 5000)]
    public async Task TychoEnables_ResolvingScopedServices_FromRequestHandlersInModules()
    {
        // Arrange
        // - no arrangement required

        // Act
        int firstResult = await _sut.ExecuteAsync(new GetModuleScopedServiceUsageRequest(), TestContext.Current.CancellationToken);
        int secondResult = await _sut.ExecuteAsync(new GetModuleScopedServiceUsageRequest(), TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(2, firstResult);
        Assert.Equal(2, secondResult);
    }

    [Fact(Timeout = 5000)]
    public async Task TychoEnables_ResolvingScopedServices_FromEventHandlersInApps()
    {
        // Arrange
        string workflowId = "event-app-scoped-workflow";
        var firstRequest = new BeginTestWorkflowRequest(new TestResult { Id = workflowId });
        var secondRequest = new BeginTestWorkflowRequest(new TestResult { Id = workflowId });

        // Act
        await _sut!.ExecuteAsync(firstRequest, TestContext.Current.CancellationToken);
        int firstResult = (await _testWorkflow.GetResult()).NumberOfCalls;
        _testWorkflow.Reset();
        await _sut!.ExecuteAsync(secondRequest, TestContext.Current.CancellationToken);
        int secondResult = (await _testWorkflow.GetResult()).NumberOfCalls;

        // Assert
        Assert.Equal(2, firstResult);
        Assert.Equal(2, secondResult);
    }

    [Fact(Timeout = 5000)]
    public async Task TychoEnables_ResolvingScopedServices_FromEventHandlersInModules()
    {
        // Arrange
        string workflowId = "event-module-scoped-workflow";
        var firstRequest = new BeginTestWorkflowRequest(new TestResult { Id = workflowId });
        var secondRequest = new BeginTestWorkflowRequest(new TestResult { Id = workflowId });

        // Act
        await _sut!.ExecuteAsync(firstRequest, TestContext.Current.CancellationToken);
        int firstResult = (await _testWorkflow.GetResult()).NumberOfCalls;
        _testWorkflow.Reset();
        await _sut!.ExecuteAsync(secondRequest, TestContext.Current.CancellationToken);
        int secondResult = (await _testWorkflow.GetResult()).NumberOfCalls;

        // Assert
        Assert.Equal(2, firstResult);
        Assert.Equal(2, secondResult);
    }

    [Fact(Timeout = 5000)]
    public async Task TychoEnables_ResolvingTransientServices_FromRequestHandlersInApps()
    {
        // Arrange
        // - no arrangement required

        // Act
        int firstResult = await _sut.ExecuteAsync(new GetAppTransientServiceUsageRequest(), TestContext.Current.CancellationToken);
        int secondResult = await _sut.ExecuteAsync(new GetAppTransientServiceUsageRequest(), TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(1, firstResult);
        Assert.Equal(1, secondResult);
    }

    [Fact(Timeout = 5000)]
    public async Task TychoEnables_ResolvingTransientServices_FromRequestHandlersInModules()
    {
        // Arrange
        // - no arrangement required

        // Act
        int firstResult = await _sut.ExecuteAsync(new GetModuleTransientServiceUsageRequest(), TestContext.Current.CancellationToken);
        int secondResult = await _sut.ExecuteAsync(new GetModuleTransientServiceUsageRequest(), TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(1, firstResult);
        Assert.Equal(1, secondResult);
    }

    [Fact(Timeout = 5000)]
    public async Task TychoEnables_ResolvingTransientServices_FromEventHandlersInApps()
    {
        // Arrange
        string workflowId = "event-app-transient-workflow";
        var firstRequest = new BeginTestWorkflowRequest(new TestResult { Id = workflowId });
        var secondRequest = new BeginTestWorkflowRequest(new TestResult { Id = workflowId });

        // Act
        await _sut!.ExecuteAsync(firstRequest, TestContext.Current.CancellationToken);
        int firstResult = (await _testWorkflow.GetResult()).NumberOfCalls;
        _testWorkflow.Reset();
        await _sut!.ExecuteAsync(secondRequest, TestContext.Current.CancellationToken);
        int secondResult = (await _testWorkflow.GetResult()).NumberOfCalls;

        // Assert
        Assert.Equal(1, firstResult);
        Assert.Equal(1, secondResult);
    }

    [Fact(Timeout = 5000)]
    public async Task TychoEnables_ResolvingTransientServices_FromEventHandlersInModules()
    {
        // Arrange
        string workflowId = "event-module-transient-workflow";
        var firstRequest = new BeginTestWorkflowRequest(new TestResult { Id = workflowId });
        var secondRequest = new BeginTestWorkflowRequest(new TestResult { Id = workflowId });

        // Act
        await _sut!.ExecuteAsync(firstRequest, TestContext.Current.CancellationToken);
        int firstResult = (await _testWorkflow.GetResult()).NumberOfCalls;
        _testWorkflow.Reset();
        await _sut!.ExecuteAsync(secondRequest, TestContext.Current.CancellationToken);
        int secondResult = (await _testWorkflow.GetResult()).NumberOfCalls;

        // Assert
        Assert.Equal(1, firstResult);
        Assert.Equal(1, secondResult);
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            await _sut.StopAsync();
        }
        finally
        {
            _sut.Dispose();
        }
    }
}
