using Tycho.Events;
using static Tycho.IntegrationTests.ForwardingEventsHorizontally.SUT.Modules.Beta.BetaModule;

namespace Tycho.IntegrationTests.ForwardingEventsHorizontally.SUT.Modules.Beta.Handlers;

internal class BetaWorkflowStartedEventHandler(IPublisher publisher) : IEventHandler<BetaWorkflowStartedEvent>
{
    private readonly IPublisher _publisher = publisher;

    public async Task HandleAsync(EventContext<BetaWorkflowStartedEvent> context, CancellationToken cancellationToken)
    {
        await _publisher.PublishAsync(new BetaWorkflowFinishedEvent(context.Payload.Result), cancellationToken);
    }
}
