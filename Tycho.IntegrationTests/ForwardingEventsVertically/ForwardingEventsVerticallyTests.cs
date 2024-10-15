using Tycho.Apps;
using Tycho.IntegrationTests._Utils;
using Tycho.IntegrationTests.ForwardingEventsVertically.SUT;

namespace Tycho.IntegrationTests.ForwardingEventsVertically;

public class ForwardingEventsVerticallyTests : IAsyncLifetime
{
    private readonly TestWorkflow<TestResult> _testWorkflow = new();
    private IApp _sut = null!;

    public async Task InitializeAsync()
    {
        _sut = await new TestApp(_testWorkflow).Run();
    }

    [Fact(Timeout = 500)]
    public async Task TychoEnables_ForwardingEvents_WithinVerticalHierarchy()
    {
        // Arrange
        var workflowId = "event-workflow";
        var request = new BeginTestWorkflowRequest(new TestResult { Id = workflowId });

        // Act
        await _sut!.Execute(request);
        var testResult = await _testWorkflow.GetResult();

        // Assert
        Assert.Equal(workflowId, testResult.Id);
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}