using Tycho.Events;
using Tycho.Events.Publishing;

namespace Tycho.IntegrationTests.ForwardingEventsHorizontally.SUT.Modules.Beta.Handlers;

internal class BetaWorkflowStartedEventHandler(IGenericPublisher publisher) : IEventHandler<BetaWorkflowStartedEvent>
{
    private readonly IGenericPublisher _publisher = publisher;

    public async Task HandleAsync(EventContext<BetaWorkflowStartedEvent> context, CancellationToken cancellationToken)
    {
        await _publisher.PublishAsync(new BetaWorkflowFinishedEvent(context.Payload.Result), cancellationToken);
    }
}