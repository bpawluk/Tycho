using Tycho.Events;
using Tycho.Events.Publishing;

namespace Tycho.IntegrationTests.ForwardingEventsVertically.SUT.Modules.Handlers;

internal class GammaWorkflowStartedEventHandler(IGenericPublisher publisher) 
    : IEventHandler<GammaWorkflowStartedEvent>
{
    private readonly IGenericPublisher _publisher = publisher;

    public async Task HandleAsync(EventContext<GammaWorkflowStartedEvent> context, CancellationToken cancellationToken)
    {
        await _publisher.PublishAsync(new GammaWorkflowFinishedEvent(context.Payload.Result), cancellationToken);
    }
}