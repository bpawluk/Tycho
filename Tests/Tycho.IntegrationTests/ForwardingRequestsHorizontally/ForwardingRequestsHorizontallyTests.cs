using Tycho.IntegrationTests._Utils;
using Tycho.IntegrationTests.ForwardingRequestsHorizontally.SUT;

namespace Tycho.IntegrationTests.ForwardingRequestsHorizontally;

public sealed class ForwardingRequestsHorizontallyTests : IAsyncLifetime
{
    private readonly TestWorkflow<TestResult> _testWorkflow = new();
    private ITestApp _sut = null!;

    public async ValueTask InitializeAsync()
    {
        _sut = await new TestApp(_testWorkflow).RunAsync();
    }

    [Fact(Timeout = 5000)]
    public async Task TychoEnables_ForwardingRequests_WithinHorizontalHierarchy()
    {
        // Arrange
        var workflowId = "request-workflow";
        var request = new Request(new TestResult { Id = workflowId });

        // Act
        await _sut!.ExecuteAsync(request, TestContext.Current.CancellationToken);
        var testResult = await _testWorkflow.GetResult();

        // Assert
        Assert.Equal(workflowId, testResult.Id);
    }

    [Fact(Timeout = 5000)]
    public async Task TychoEnables_ForwardingRequestsForResponses_WithinHorizontalHierarchy()
    {
        // Arrange
        var workflowId = "request-with-response-workflow";
        var message = new RequestWithResponse(new TestResult { Id = workflowId });

        // Act
        var response = await _sut!.ExecuteAsync(message, TestContext.Current.CancellationToken);
        var testResult = await _testWorkflow.GetResult();

        // Assert
        Assert.Equal("Test = Passed", response);
        Assert.Equal(workflowId, testResult.Id);
    }

    [Fact(Timeout = 5000)]
    public async Task TychoEnables_ForwardingMappedRequests_WithinHorizontalHierarchy()
    {
        // Arrange
        var workflowId = "mapped-request-workflow";
        var request = new RequestToMap(new TestResult { Id = workflowId });

        // Act
        await _sut!.ExecuteAsync(request, TestContext.Current.CancellationToken);
        var testResult = await _testWorkflow.GetResult();

        // Assert
        Assert.Equal(workflowId, testResult.Id);
    }

    [Fact(Timeout = 5000)]
    public async Task TychoEnables_ForwardingMappedRequestsForResponses_WithinHorizontalHierarchy()
    {
        // Arrange
        var workflowId = "mapped-request-with-response-workflow";
        var message = new RequestToMapWithResponse(new TestResult { Id = workflowId });

        // Act
        var response = await _sut!.ExecuteAsync(message, TestContext.Current.CancellationToken);
        var testResult = await _testWorkflow.GetResult();

        // Assert
        Assert.NotNull(response);
        Assert.Equal("Test = Passed", response.Value);
        Assert.Equal(workflowId, testResult.Id);
    }

    public async ValueTask DisposeAsync()
    {
        await _sut!.DisposeAsync();
    }
}