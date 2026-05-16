using Tycho.IntegrationTests.ForwardingEventsVertically.SUT;
using Tycho.IntegrationTests._Utils;

namespace Tycho.IntegrationTests.ForwardingEventsVertically;

public sealed class ForwardingEventsVerticallyTests : IAsyncLifetime
{
    private readonly TestWorkflow<TestResult> _testWorkflow = new();
    private ITestApp _sut = null!;

    public async ValueTask InitializeAsync()
    {
        _sut = await new TestApp(_testWorkflow).RunAsync();
    }

    [Fact(Timeout = 5000)]
    public async Task TychoEnables_ForwardingEvents_WithinVerticalHierarchy()
    {
        // Arrange
        string workflowId = "event-workflow";
        var request = new BeginTestWorkflowRequest(new TestResult { Id = workflowId });

        // Act
        await _sut!.ExecuteAsync(request, TestContext.Current.CancellationToken);
        TestResult testResult = await _testWorkflow.GetResult();

        // Assert
        Assert.Equal(workflowId, testResult.Id);
    }

    [Fact(Timeout = 5000)]
    public async Task TychoEnables_ForwardingMappedEvents_WithinVerticalHierarchy()
    {
        // Arrange
        string workflowId = "mapped-event-workflow";
        var request = new BeginTestWorkflowRequest(new TestResult { Id = workflowId });

        // Act
        await _sut!.ExecuteAsync(request, TestContext.Current.CancellationToken);
        TestResult testResult = await _testWorkflow.GetResult();

        // Assert
        Assert.Equal(workflowId, testResult.Id);
    }

    public async ValueTask DisposeAsync()
    {
        await _sut!.DisposeAsync();
    }
}
