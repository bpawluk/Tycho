using Tycho.Events;

namespace Tycho.IntegrationTests.ForwardingEventsHorizontally.SUT.Modules.Beta.Handlers;

internal class BetaWorkflowStartedEventHandler(IEventPublisher publisher) : IEventHandler<BetaWorkflowStartedEvent>
{
    private readonly IEventPublisher _publisher = publisher;

    public async Task Handle(BetaWorkflowStartedEvent eventData, CancellationToken cancellationToken)
    {
        await _publisher.Publish(new BetaWorkflowFinishedEvent(eventData.Result), cancellationToken);
    }
}