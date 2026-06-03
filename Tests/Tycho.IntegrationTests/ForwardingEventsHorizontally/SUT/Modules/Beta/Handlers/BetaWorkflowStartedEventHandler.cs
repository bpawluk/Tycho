using Tycho.Events;

namespace Tycho.IntegrationTests.ForwardingEventsHorizontally.SUT.Modules.Beta.Handlers;

internal class BetaWorkflowStartedEventHandler(IBetaModule.IPublisher publisher) : IEventHandler<BetaWorkflowStartedEvent>
{
    private readonly IBetaModule.IPublisher _publisher = publisher;

    public async Task HandleAsync(EventContext<BetaWorkflowStartedEvent> context, CancellationToken cancellationToken)
    {
        await _publisher.PublishAsync(new BetaWorkflowFinishedEvent(context.Payload.Result), cancellationToken);
    }
}
