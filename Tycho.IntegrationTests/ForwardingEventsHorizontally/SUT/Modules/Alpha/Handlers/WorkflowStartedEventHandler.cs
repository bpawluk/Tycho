using TychoV2.Events;

namespace Tycho.IntegrationTests.ForwardingEventsHorizontally.SUT.Modules.Alpha.Handlers;

internal class WorkflowStartedEventHandler(IEventPublisher publisher) : IEventHandler<WorkflowStartedEvent>
{
    private readonly IEventPublisher _publisher = publisher;

    public async Task Handle(WorkflowStartedEvent eventData, CancellationToken cancellationToken)
    {
        await _publisher.Publish(new WorkflowFinishedEvent(eventData.Result, typeof(AlphaModule)), cancellationToken);
    }
}
