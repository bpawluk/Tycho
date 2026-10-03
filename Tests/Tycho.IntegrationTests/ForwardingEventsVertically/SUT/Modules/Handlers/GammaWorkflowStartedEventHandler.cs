using Tycho.Events;

namespace Tycho.IntegrationTests.ForwardingEventsVertically.SUT.Modules.Handlers;

internal class GammaWorkflowStartedEventHandler(IGammaModulePublisher publisher)
    : IEventHandler<GammaWorkflowStartedEvent>
{
    private readonly IGammaModulePublisher _publisher = publisher;

    public async Task HandleAsync(EventContext<GammaWorkflowStartedEvent> context, CancellationToken cancellationToken)
    {
        TestResult result = context.Payload.Result with { LeafValue = context.Payload.Result.Value };
        await _publisher.PublishAsync(new GammaWorkflowFinishedEvent(result), cancellationToken);
    }
}
