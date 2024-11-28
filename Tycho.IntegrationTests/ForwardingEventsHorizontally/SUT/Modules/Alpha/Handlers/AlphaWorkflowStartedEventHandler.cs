using Tycho.Events;

namespace Tycho.IntegrationTests.ForwardingEventsHorizontally.SUT.Modules.Alpha.Handlers;

internal class AlphaWorkflowStartedEventHandler(IEventPublisher publisher) : IEventHandler<AlphaWorkflowStartedEvent>
{
    private readonly IEventPublisher _publisher = publisher;

    public async Task Handle(AlphaWorkflowStartedEvent eventData, CancellationToken cancellationToken)
    {
        await _publisher.Publish(new AlphaWorkflowFinishedEvent(eventData.Result), cancellationToken);
    }
}