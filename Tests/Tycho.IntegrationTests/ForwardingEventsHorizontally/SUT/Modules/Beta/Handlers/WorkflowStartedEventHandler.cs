using Tycho.Events;

namespace Tycho.IntegrationTests.ForwardingEventsHorizontally.SUT.Modules.Beta.Handlers;

internal class WorkflowStartedEventHandler(IBetaModule.IPublisher publisher) : IEventHandler<WorkflowStartedEvent>
{
    private readonly IBetaModule.IPublisher _publisher = publisher;

    public async Task HandleAsync(EventContext<WorkflowStartedEvent> context, CancellationToken cancellationToken)
    {
        await _publisher.PublishAsync(new WorkflowFinishedEvent(context.Payload.Result, nameof(BetaModule)), cancellationToken);
    }
}
