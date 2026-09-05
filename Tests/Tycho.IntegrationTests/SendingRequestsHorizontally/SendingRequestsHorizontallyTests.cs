using Tycho.IntegrationTests.SendingRequestsHorizontally.SUT;
using Tycho.IntegrationTests._Utils;

namespace Tycho.IntegrationTests.SendingRequestsHorizontally;

public sealed class SendingRequestsHorizontallyTests : IAsyncLifetime
{
    private readonly TestWorkflow<TestResult> _testWorkflow = new();
    private ITestApp _sut = null!;

    public async ValueTask InitializeAsync()
    {
        _sut = new TestApp(_testWorkflow).CreateAppBuilder().Build();
        await _sut.StartAsync(TestContext.Current.CancellationToken);
    }

    [Fact(Timeout = 5000)]
    public async Task TychoEnables_SendingRequests_WithinHorizontalHierarchy()
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
    public async Task TychoEnables_SendingRequestsForResponses_WithinHorizontalHierarchy()
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
