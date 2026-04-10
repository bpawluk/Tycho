using Tycho.IntegrationTests._Utils;
using Tycho.IntegrationTests.SendingRequestsHorizontally.SUT;

namespace Tycho.IntegrationTests.SendingRequestsHorizontally;

public class SendingRequestsHorizontallyTests : IAsyncLifetime
{
    private readonly TestWorkflow<TestResult> _testWorkflow = new();
    private ITestApp _sut = null!;

    public async ValueTask InitializeAsync()
    {
        _sut = await new TestApp(_testWorkflow).RunAsync();
    }

    [Fact(Timeout = 500)]
    public async Task TychoEnables_SendingRequests_WithinHorizontalHierarchy()
    {
        // Arrange
        var workflowId = "request-workflow";
        var request = new Request(new TestResult { Id = workflowId });

        // Act
        await _sut!.ExecuteAsync(request);
        var testResult = await _testWorkflow.GetResult();

        // Assert
        Assert.Equal(workflowId, testResult.Id);
        Assert.Equal(7, testResult.HandlingCount);
    }

    [Fact(Timeout = 500)]
    public async Task TychoEnables_SendingRequestsForResponses_WithinHorizontalHierarchy()
    {
        // Arrange
        var workflowId = "request-with-response-workflow";
        var message = new RequestWithResponse(new TestResult { Id = workflowId });

        // Act
        var response = await _sut!.ExecuteAsync(message);
        var testResult = await _testWorkflow.GetResult();

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