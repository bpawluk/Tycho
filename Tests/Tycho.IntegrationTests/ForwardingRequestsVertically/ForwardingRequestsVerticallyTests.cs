using Tycho.IntegrationTests.ForwardingRequestsVertically.SUT;
using Tycho.IntegrationTests._Utils;

namespace Tycho.IntegrationTests.ForwardingRequestsVertically;

public sealed class ForwardingRequestsVerticallyTests : IAsyncLifetime
{
    private readonly TestWorkflow<TestResult> _testWorkflow = new();
    private ITestApp _sut = null!;

    public async ValueTask InitializeAsync()
    {
        _sut = new TestApp(_testWorkflow).CreateAppBuilder().Build();
        await _sut.StartAsync(TestContext.Current.CancellationToken);
    }

    [Fact(Timeout = 5000)]
    public async Task TychoEnables_ForwardingRequests_WithinVerticalHierarchy()
    {
        // Arrange
        string workflowId = "request-workflow";
        var request = new Request(new TestResult { Id = workflowId });

        // Act
        await _sut!.ExecuteAsync(request, TestContext.Current.CancellationToken);
        TestResult testResult = await _testWorkflow.GetResult();

        // Assert
        Assert.Equal(workflowId, testResult.Id);
    }

    [Fact(Timeout = 5000)]
    public async Task TychoEnables_ForwardingRequestsForResponses_WithinVerticalHierarchy()
    {
        // Arrange
        string workflowId = "request-with-response-workflow";
        var message = new RequestWithResponse(new TestResult { Id = workflowId });

        // Act
        string response = await _sut!.ExecuteAsync(message, TestContext.Current.CancellationToken);
        TestResult testResult = await _testWorkflow.GetResult();

        // Assert
        Assert.Equal("Test = Passed", response);
        Assert.Equal(workflowId, testResult.Id);
    }

    [Fact(Timeout = 5000)]
    public async Task TychoEnables_ForwardingMappedRequests_WithinVerticalHierarchy()
    {
        // Arrange
        string workflowId = "mapped-request-workflow";
        var request = new RequestToMap(new TestResult { Id = workflowId });

        // Act
        await _sut!.ExecuteAsync(request, TestContext.Current.CancellationToken);
        TestResult testResult = await _testWorkflow.GetResult();

        // Assert
        Assert.Equal(workflowId, testResult.Id);
    }

    [Fact(Timeout = 5000)]
    public async Task TychoEnables_ForwardingMappedRequestsForResponses_WithinVerticalHierarchy()
    {
        // Arrange
        string workflowId = "mapped-request-with-response-workflow";
        var message = new RequestToMapWithResponse(new TestResult { Id = workflowId });

        // Act
        RequestToMapWithResponse.Response response = await _sut!.ExecuteAsync(message, TestContext.Current.CancellationToken);
        TestResult testResult = await _testWorkflow.GetResult();

        // Assert
        Assert.NotNull(response);
        Assert.Equal("Test = Passed", response.Value);
        Assert.Equal(workflowId, testResult.Id);
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
