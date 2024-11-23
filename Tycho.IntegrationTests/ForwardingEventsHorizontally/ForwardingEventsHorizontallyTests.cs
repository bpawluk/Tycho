using Tycho.IntegrationTests._Utils;
using Tycho.IntegrationTests.ForwardingEventsHorizontally.SUT;
using Tycho.Structure;

namespace Tycho.IntegrationTests.ForwardingEventsHorizontally;

public class ForwardingEventsHorizontallyTests : IAsyncLifetime
{
    private readonly TestWorkflow<TestResult> _testWorkflow = new();
    private IApp _sut = null!;

    public async Task InitializeAsync()
    {
        _sut = await new TestApp(_testWorkflow).Run();
    }

    [Fact(Timeout = 500)]
    public async Task TychoEnables_ForwardingEvents_WithinHorizontalHierarchy()
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

    [Fact(Timeout = 500)]
    public async Task TychoEnables_ForwardingMappedEvents_WithinHorizontalHierarchy()
    {
        // Arrange
        var workflowId = "mapped-event-workflow";
        var request = new BeginTestWorkflowRequest(new TestResult { Id = workflowId });

        // Act
        await _sut!.Execute(request);
        var testResult = await _testWorkflow.GetResult();

        // Assert
        Assert.Equal(workflowId, testResult.Id);
    }

    public async Task DisposeAsync()
    {
        await _sut!.DisposeAsync();
    }
}