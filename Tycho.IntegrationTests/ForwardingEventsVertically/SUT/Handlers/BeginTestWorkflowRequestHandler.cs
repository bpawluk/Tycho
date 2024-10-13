using Tycho.Events;
using Tycho.Requests;

namespace Tycho.IntegrationTests.ForwardingEventsVertically.SUT.Handlers;

internal class BeginTestWorkflowRequestHandler(IEventPublisher publisher) : IRequestHandler<BeginTestWorkflowRequest>
{
    private readonly IEventPublisher _publisher = publisher;

    public async Task Handle(BeginTestWorkflowRequest requestData, CancellationToken cancellationToken)
    {
        await _publisher.Publish(new WorkflowStartedEvent(requestData.Result), cancellationToken);
    }
}
