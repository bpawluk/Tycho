using Tycho.IntegrationTests._Utils;
using Tycho.IntegrationTests.SendingRequestsVertically.SUT;
using Tycho.Structure;

namespace Tycho.IntegrationTests.SendingRequestsVertically;

public class SendingRequestsVerticallyTests : IAsyncLifetime
{
    private readonly TestWorkflow<TestResult> _testWorkflow = new();
    private IApp _sut = null!;

    public async Task InitializeAsync()
    {
        _sut = await new TestApp(_testWorkflow).Run();
    }

    [Fact(Timeout = 500)]
    public async Task TychoEnables_SendingRequests_WithinVerticalHierarchy()
    {
        // Arrange
        var workflowId = "request-workflow";
        var request = new Request(new TestResult { Id = workflowId });

        // Act
        await _sut!.Execute(request);
        var testResult = await _testWorkflow.GetResult();

        // Assert
        Assert.Equal(workflowId, testResult.Id);
        Assert.Equal(7, testResult.HandlingCount);
    }

    [Fact(Timeout = 500)]
    public async Task TychoEnables_SendingRequestsForResponses_WithinVerticalHierarchy()
    {
        // Arrange
        var workflowId = "request-with-response-workflow";
        var message = new RequestWithResponse(new TestResult { Id = workflowId });

        // Act
        var response = await _sut!.Execute<RequestWithResponse, string>(message);
        var testResult = await _testWorkflow.GetResult();

        // Assert
        Assert.Equal("Test = Passed", response);
        Assert.Equal(workflowId, testResult.Id);
        Assert.Equal(7, testResult.HandlingCount);
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}