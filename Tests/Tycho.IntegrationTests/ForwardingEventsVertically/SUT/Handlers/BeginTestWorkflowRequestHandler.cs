using Tycho.Events.Publishing;
using Tycho.Requests;

namespace Tycho.IntegrationTests.ForwardingEventsVertically.SUT.Handlers;

internal class BeginTestWorkflowRequestHandler(IGenericPublisher publisher) 
    : IRequestHandler<BeginTestWorkflowRequest>
{
    private readonly IGenericPublisher _publisher = publisher;

    public async Task HandleAsync(BeginTestWorkflowRequest requestData, CancellationToken cancellationToken)
    {
        if (requestData.Result.Id == "event-workflow")
        {
            await _publisher.PublishAsync(new WorkflowStartedEvent(requestData.Result), cancellationToken);
        }
        else if (requestData.Result.Id == "mapped-event-workflow")
        {
            await _publisher.PublishAsync(new WorkflowWithMappingStartedEvent(requestData.Result), cancellationToken);
        }
        else
        {
            throw new ArgumentException($"Unknown workflow ID {requestData.Result.Id}");
        }
    }
}