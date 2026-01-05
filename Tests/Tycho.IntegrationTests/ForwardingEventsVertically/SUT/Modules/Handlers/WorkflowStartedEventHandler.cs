using Tycho.Events;

namespace Tycho.IntegrationTests.ForwardingEventsVertically.SUT.Modules.Handlers;

internal class WorkflowStartedEventHandler(IEventPublisher publisher) 
    : IEventHandler<WorkflowStartedEvent>
{
    private readonly IEventPublisher _publisher = publisher;

    public async Task HandleAsync(EventContext<WorkflowStartedEvent> context, CancellationToken cancellationToken)
    {
        await _publisher.PublishAsync(new WorkflowFinishedEvent(context.Payload.Result), cancellationToken);
    }
}