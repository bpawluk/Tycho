using Tycho.Events;

namespace Tycho.IntegrationTests.ForwardingEventsHorizontally.SUT.Modules.Gamma.Handlers;

internal class WorkflowStartedEventHandler(IEventPublisher publisher) : IEventHandler<WorkflowStartedEvent>
{
    private readonly IEventPublisher _publisher = publisher;

    public async Task Handle(WorkflowStartedEvent eventData, CancellationToken cancellationToken)
    {
        await _publisher.Publish(new WorkflowFinishedEvent(eventData.Result, typeof(GammaModule)), cancellationToken);
    }
}