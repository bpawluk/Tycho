using Tycho.IntegrationTests.SendingRequestsHorizontally.SUT;
using Tycho.IntegrationTests.TestUtils;
using TychoV2.Apps;

namespace Tycho.IntegrationTests.SendingRequestsHorizontally;

public class SendingRequestsHorizontallyTests : IAsyncLifetime
{
    private readonly TestWorkflow<TestResult> _testWorkflow = new();
    private IApp _sut = null!;

    public async Task InitializeAsync()
    {
        _sut = await new TestApp(_testWorkflow).Run();
    }

    [Fact]
    public async Task TychoEnables_SendingRequests_WithinHorizontalHierarchy()
    {
        // Arrange
        var workflowId = "request-workflow";
        var request = new Request(new() { Id = workflowId });

        // Act
        await _sut!.Execute(request);
        var testResult = await _testWorkflow.GetResult();

        // Assert
        Assert.Equal(workflowId, testResult.Id);
        Assert.Equal(7, testResult.HandlingCount);
    }

    [Fact]
    public async Task TychoEnables_SendingRequestsForResponses_WithinHorizontalHierarchy()
    {
        // Arrange
        var workflowId = "request-with-response-workflow";
        var message = new RequestWithResponse(new() { Id = workflowId });

        // Act
        var response = await _sut!.Execute<RequestWithResponse, string>(message);
        var testResult = await _testWorkflow.GetResult();

        // Assert
        Assert.Equal("Test = Passed", response);
        Assert.Equal(workflowId, testResult.Id);
        Assert.Equal(7, testResult.HandlingCount);
    }

    public Task DisposeAsync() => Task.CompletedTask;
}
