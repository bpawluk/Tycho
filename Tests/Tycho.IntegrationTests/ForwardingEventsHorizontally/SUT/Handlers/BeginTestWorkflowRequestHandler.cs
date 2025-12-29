using Tycho.Events;
using Tycho.Requests;

namespace Tycho.IntegrationTests.ForwardingEventsHorizontally.SUT.Handlers;

internal class BeginTestWorkflowRequestHandler(IEventPublisher publisher) 
    : IRequestHandler<BeginTestWorkflowRequest>
{
    private readonly IEventPublisher _publisher = publisher;

    public async Task Handle(BeginTestWorkflowRequest requestData, CancellationToken cancellationToken)
    {
        if (requestData.Result.Id == "event-workflow")
        {
            await _publisher.Publish(new WorkflowStartedEvent(requestData.Result), cancellationToken);
        }
        else if (requestData.Result.Id == "mapped-event-workflow")
        {
            await _publisher.Publish(new WorkflowWithMappingStartedEvent(requestData.Result), cancellationToken);
        }
        else
        {
            throw new ArgumentException($"Unknown workflow ID {requestData.Result.Id}");
        }
    }
}