using Tycho.Events;

namespace Tycho.IntegrationTests.ForwardingEventsHorizontally.SUT.Modules.Beta.Handlers;

internal class BetaWorkflowStartedEventHandler(IEventPublisher publisher) : IEventHandler<BetaWorkflowStartedEvent>
{
    private readonly IEventPublisher _publisher = publisher;

    public async Task HandleAsync(EventContext<BetaWorkflowStartedEvent> context, CancellationToken cancellationToken)
    {
        await _publisher.PublishAsync(new BetaWorkflowFinishedEvent(context.Payload.Result), cancellationToken);
    }
}