using Tycho.Events;

namespace Tycho.IntegrationTests.ForwardingEventsHorizontally.SUT.Modules.Beta.Handlers;

internal class WorkflowStartedEventHandler(IEventPublisher publisher) : IEventHandler<WorkflowStartedEvent>
{
    private readonly IEventPublisher _publisher = publisher;

    public async Task Handle(EventContext<WorkflowStartedEvent> context, CancellationToken cancellationToken)
    {
        await _publisher.Publish(new WorkflowFinishedEvent(context.Payload.Result, typeof(BetaModule)), cancellationToken);
    }
}