using Tycho.IntegrationTests._Utils;
using Tycho.IntegrationTests.ForwardingRequestsVertically.SUT;

namespace Tycho.IntegrationTests.ForwardingRequestsVertically;

public sealed class ForwardingRequestsVerticallyTests : IAsyncLifetime
{
    private readonly TestWorkflow<TestResult> _testWorkflow = new();
    private ITestApp _sut = null!;

    public async ValueTask InitializeAsync()
    {
        _sut = await new TestApp(_testWorkflow).RunAsync();
    }

    [Fact(Timeout = 500)]
    public async Task TychoEnables_ForwardingRequests_WithinVerticalHierarchy()
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

    [Fact(Timeout = 500)]
    public async Task TychoEnables_ForwardingRequestsForResponses_WithinVerticalHierarchy()
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

    [Fact(Timeout = 500)]
    public async Task TychoEnables_ForwardingMappedRequests_WithinVerticalHierarchy()
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

    [Fact(Timeout = 500)]
    public async Task TychoEnables_ForwardingMappedRequestsForResponses_WithinVerticalHierarchy()
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