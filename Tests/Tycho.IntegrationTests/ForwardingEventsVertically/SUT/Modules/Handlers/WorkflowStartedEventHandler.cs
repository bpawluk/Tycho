using Tycho.Events;
using Tycho.Events.Publishing;

namespace Tycho.IntegrationTests.ForwardingEventsVertically.SUT.Modules.Handlers;

internal class WorkflowStartedEventHandler(IGenericPublisher publisher) 
    : IEventHandler<WorkflowStartedEvent>
{
    private readonly IGenericPublisher _publisher = publisher;

    public async Task HandleAsync(EventContext<WorkflowStartedEvent> context, CancellationToken cancellationToken)
    {
        await _publisher.PublishAsync(new WorkflowFinishedEvent(context.Payload.Result), cancellationToken);
    }
}