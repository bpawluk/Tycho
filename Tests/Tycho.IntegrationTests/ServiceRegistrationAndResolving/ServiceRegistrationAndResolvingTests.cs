using Tycho.IntegrationTests._Utils;
using Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT;
using Tycho.IntegrationTests.ServiceRegistrationAndResolving.SUT.Modules;

namespace Tycho.IntegrationTests.ServiceRegistrationAndResolving;

public class ServiceRegistrationAndResolvingTests : IAsyncLifetime
{
    private readonly TestWorkflow<TestResult> _testWorkflow = new();
    private ITestApp _sut = null!;

    public async Task InitializeAsync()
    {
        _sut = await new TestApp(_testWorkflow).RunAsync();
    }

    [Fact(Timeout = 500)]
    public async Task TychoEnables_ResolvingSingletonServices_FromRequestHandlersInApps()
    {
        // Arrange
        // - no arrangement required

        // Act
        var firstResult = await _sut.ExecuteAsync(new GetAppSingletonServiceUsageRequest());
        var secondResult =  await _sut.ExecuteAsync(new GetAppSingletonServiceUsageRequest());

        // Assert
        Assert.Equal(2, firstResult);
        Assert.Equal(4, secondResult);
    }

    [Fact(Timeout = 500)]
    public async Task TychoEnables_ResolvingSingletonServices_FromRequestHandlersInModules()
    {
        // Arrange
        // - no arrangement required

        // Act
        var firstResult = await _sut.ExecuteAsync(new GetModuleSingletonServiceUsageRequest());
        var secondResult = await _sut.ExecuteAsync(new GetModuleSingletonServiceUsageRequest());

        // Assert
        Assert.Equal(2, firstResult);
        Assert.Equal(4, secondResult);
    }

    [Fact(Timeout = 500)]
    public async Task TychoEnables_ResolvingSingletonServices_FromEventHandlersInApps()
    {
        // Arrange
        var workflowId = "event-app-singleton-workflow";
        var firstRequest = new BeginTestWorkflowRequest(new TestResult { Id = workflowId });
        var secondRequest = new BeginTestWorkflowRequest(new TestResult { Id = workflowId });

        // Act
        await _sut!.ExecuteAsync(firstRequest);
        var firstResult = (await _testWorkflow.GetResult()).NumberOfCalls;
        _testWorkflow.Reset();
        await _sut!.ExecuteAsync(secondRequest);
        var secondResult = (await _testWorkflow.GetResult()).NumberOfCalls;

        // Assert
        Assert.Equal(2, firstResult);
        Assert.Equal(4, secondResult);
    }

    [Fact(Timeout = 500)]
    public async Task TychoEnables_ResolvingSingletonServices_FromEventHandlersInModules()
    {
        // Arrange
        var workflowId = "event-module-singleton-workflow";
        var firstRequest = new BeginTestWorkflowRequest(new TestResult { Id = workflowId });
        var secondRequest = new BeginTestWorkflowRequest(new TestResult { Id = workflowId });

        // Act
        await _sut!.ExecuteAsync(firstRequest);
        var firstResult = (await _testWorkflow.GetResult()).NumberOfCalls;
        _testWorkflow.Reset();
        await _sut!.ExecuteAsync(secondRequest);
        var secondResult = (await _testWorkflow.GetResult()).NumberOfCalls;

        // Assert
        Assert.Equal(2, firstResult);
        Assert.Equal(4, secondResult);
    }

    [Fact(Timeout = 500)]
    public async Task TychoEnables_ResolvingScopedServices_FromRequestHandlersInApps()
    {
        // Arrange
        // - no arrangement required

        // Act
        var firstResult = await _sut.ExecuteAsync(new GetAppScopedServiceUsageRequest());
        var secondResult = await _sut.ExecuteAsync(new GetAppScopedServiceUsageRequest());

        // Assert
        Assert.Equal(2, firstResult);
        Assert.Equal(2, secondResult);
    }

    [Fact(Timeout = 500)]
    public async Task TychoEnables_ResolvingScopedServices_FromRequestHandlersInModules()
    {
        // Arrange
        // - no arrangement required

        // Act
        var firstResult = await _sut.ExecuteAsync(new GetModuleScopedServiceUsageRequest());
        var secondResult = await _sut.ExecuteAsync(new GetModuleScopedServiceUsageRequest());

        // Assert
        Assert.Equal(2, firstResult);
        Assert.Equal(2, secondResult);
    }

    [Fact(Timeout = 500)]
    public async Task TychoEnables_ResolvingScopedServices_FromEventHandlersInApps()
    {
        // Arrange
        var workflowId = "event-app-scoped-workflow";
        var firstRequest = new BeginTestWorkflowRequest(new TestResult { Id = workflowId });
        var secondRequest = new BeginTestWorkflowRequest(new TestResult { Id = workflowId });

        // Act
        await _sut!.ExecuteAsync(firstRequest);
        var firstResult = (await _testWorkflow.GetResult()).NumberOfCalls;
        _testWorkflow.Reset();
        await _sut!.ExecuteAsync(secondRequest);
        var secondResult = (await _testWorkflow.GetResult()).NumberOfCalls;

        // Assert
        Assert.Equal(2, firstResult);
        Assert.Equal(2, secondResult);
    }

    [Fact(Timeout = 500)]
    public async Task TychoEnables_ResolvingScopedServices_FromEventHandlersInModules()
    {
        // Arrange
        var workflowId = "event-module-scoped-workflow";
        var firstRequest = new BeginTestWorkflowRequest(new TestResult { Id = workflowId });
        var secondRequest = new BeginTestWorkflowRequest(new TestResult { Id = workflowId });

        // Act
        await _sut!.ExecuteAsync(firstRequest);
        var firstResult = (await _testWorkflow.GetResult()).NumberOfCalls;
        _testWorkflow.Reset();
        await _sut!.ExecuteAsync(secondRequest);
        var secondResult = (await _testWorkflow.GetResult()).NumberOfCalls;

        // Assert
        Assert.Equal(2, firstResult);
        Assert.Equal(2, secondResult);
    }

    [Fact(Timeout = 500)]
    public async Task TychoEnables_ResolvingTransientServices_FromRequestHandlersInApps()
    {
        // Arrange
        // - no arrangement required

        // Act
        var firstResult = await _sut.ExecuteAsync(new GetAppTransientServiceUsageRequest());
        var secondResult = await _sut.ExecuteAsync(new GetAppTransientServiceUsageRequest());

        // Assert
        Assert.Equal(1, firstResult);
        Assert.Equal(1, secondResult);
    }

    [Fact(Timeout = 500)]
    public async Task TychoEnables_ResolvingTransientServices_FromRequestHandlersInModules()
    {
        // Arrange
        // - no arrangement required

        // Act
        var firstResult = await _sut.ExecuteAsync(new GetModuleTransientServiceUsageRequest());
        var secondResult = await _sut.ExecuteAsync(new GetModuleTransientServiceUsageRequest());

        // Assert
        Assert.Equal(1, firstResult);
        Assert.Equal(1, secondResult);
    }

    [Fact(Timeout = 500)]
    public async Task TychoEnables_ResolvingTransientServices_FromEventHandlersInApps()
    {
        // Arrange
        var workflowId = "event-app-transient-workflow";
        var firstRequest = new BeginTestWorkflowRequest(new TestResult { Id = workflowId });
        var secondRequest = new BeginTestWorkflowRequest(new TestResult { Id = workflowId });

        // Act
        await _sut!.ExecuteAsync(firstRequest);
        var firstResult = (await _testWorkflow.GetResult()).NumberOfCalls;
        _testWorkflow.Reset();
        await _sut!.ExecuteAsync(secondRequest);
        var secondResult = (await _testWorkflow.GetResult()).NumberOfCalls;

        // Assert
        Assert.Equal(1, firstResult);
        Assert.Equal(1, secondResult);
    }

    [Fact(Timeout = 500)]
    public async Task TychoEnables_ResolvingTransientServices_FromEventHandlersInModules()
    {
        // Arrange
        var workflowId = "event-module-transient-workflow";
        var firstRequest = new BeginTestWorkflowRequest(new TestResult { Id = workflowId });
        var secondRequest = new BeginTestWorkflowRequest(new TestResult { Id = workflowId });

        // Act
        await _sut!.ExecuteAsync(firstRequest);
        var firstResult = (await _testWorkflow.GetResult()).NumberOfCalls;
        _testWorkflow.Reset();
        await _sut!.ExecuteAsync(secondRequest);
        var secondResult = (await _testWorkflow.GetResult()).NumberOfCalls;

        // Assert
        Assert.Equal(1, firstResult);
        Assert.Equal(1, secondResult);
    }

    public async Task DisposeAsync()
    {
        await _sut!.DisposeAsync();
    }
}