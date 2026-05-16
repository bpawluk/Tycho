using Tycho.Events;
using static Tycho.IntegrationTests.ForwardingEventsVertically.SUT.Modules.GammaModule;

namespace Tycho.IntegrationTests.ForwardingEventsVertically.SUT.Modules.Handlers;

internal class GammaWorkflowStartedEventHandler(IPublisher publisher)
    : IEventHandler<GammaWorkflowStartedEvent>
{
    private readonly IPublisher _publisher = publisher;

    public async Task HandleAsync(EventContext<GammaWorkflowStartedEvent> context, CancellationToken cancellationToken)
    {
        await _publisher.PublishAsync(new GammaWorkflowFinishedEvent(context.Payload.Result), cancellationToken);
    }
}
