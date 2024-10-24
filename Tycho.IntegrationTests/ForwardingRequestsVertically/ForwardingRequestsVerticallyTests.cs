﻿using Tycho.IntegrationTests._Utils;
using Tycho.IntegrationTests.ForwardingRequestsVertically.SUT;
using Tycho.Structure;
using static Tycho.IntegrationTests.ForwardingRequestsVertically.SUT.RequestToMapWithResponse;

namespace Tycho.IntegrationTests.ForwardingRequestsVertically;

public class ForwardingRequestsVerticallyTests : IAsyncLifetime
{
    private readonly TestWorkflow<TestResult> _testWorkflow = new();
    private IApp _sut = null!;

    public async Task InitializeAsync()
    {
        _sut = await new TestApp(_testWorkflow).Run();
    }

    [Fact(Timeout = 500)]
    public async Task TychoEnables_ForwardingRequests_WithinVerticalHierarchy()
    {
        // Arrange
        var workflowId = "request-workflow";
        var request = new Request(new TestResult { Id = workflowId });

        // Act
        await _sut!.Execute(request);
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
        var response = await _sut!.Execute<RequestWithResponse, string>(message);
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
        await _sut!.Execute(request);
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
        var response = await _sut!.Execute<RequestToMapWithResponse, Response>(message);
        var testResult = await _testWorkflow.GetResult();

        // Assert
        Assert.NotNull(response);
        Assert.Equal("Test = Passed", response.Value);
        Assert.Equal(workflowId, testResult.Id);
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}