using Tycho.IntegrationTests.SendingRequestsVertically.SUT;
using Tycho.IntegrationTests._Utils;

namespace Tycho.IntegrationTests.SendingRequestsVertically;

public sealed class SendingRequestsVerticallyTests : IAsyncLifetime
{
    private readonly TestWorkflow<TestResult> _testWorkflow = new();
    private ITestApp _sut = null!;

    public async ValueTask InitializeAsync()
    {
        _sut = await new TestApp(_testWorkflow).RunAsync();
    }

    [Fact(Timeout = 5000)]
    public async Task TychoEnables_SendingRequests_WithinVerticalHierarchy()
    {
        // Arrange
        string workflowId = "request-workflow";
        var request = new Request(new TestResult { Id = workflowId });

        // Act
        await _sut!.ExecuteAsync(request, TestContext.Current.CancellationToken);
        TestResult testResult = await _testWorkflow.GetResult();

        // Assert
        Assert.Equal(workflowId, testResult.Id);
        Assert.Equal(7, testResult.HandlingCount);
    }

    [Fact(Timeout = 5000)]
    public async Task TychoEnables_SendingRequestsForResponses_WithinVerticalHierarchy()
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
        Assert.Equal(7, testResult.HandlingCount);
    }

    public async ValueTask DisposeAsync()
    {
        await _sut!.DisposeAsync();
    }
}
