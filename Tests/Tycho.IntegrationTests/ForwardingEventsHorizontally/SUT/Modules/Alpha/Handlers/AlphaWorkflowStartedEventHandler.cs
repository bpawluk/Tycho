using Tycho.Events;

namespace Tycho.IntegrationTests.ForwardingEventsHorizontally.SUT.Modules.Alpha.Handlers;

internal class AlphaWorkflowStartedEventHandler(IEventPublisher publisher) : IEventHandler<AlphaWorkflowStartedEvent>
{
    private readonly IEventPublisher _publisher = publisher;

    public async Task HandleAsync(EventContext<AlphaWorkflowStartedEvent> context, CancellationToken cancellationToken)
    {
        await _publisher.PublishAsync(new AlphaWorkflowFinishedEvent(context.Payload.Result), cancellationToken);
    }
}