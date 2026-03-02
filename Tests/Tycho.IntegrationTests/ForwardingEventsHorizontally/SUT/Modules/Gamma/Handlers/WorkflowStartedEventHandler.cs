using Tycho.Events;
using static Tycho.IntegrationTests.ForwardingEventsHorizontally.SUT.Modules.Gamma.GammaModule;

namespace Tycho.IntegrationTests.ForwardingEventsHorizontally.SUT.Modules.Gamma.Handlers;

internal class WorkflowStartedEventHandler(IPublisher publisher) : IEventHandler<WorkflowStartedEvent>
{
    private readonly IPublisher _publisher = publisher;

    public async Task HandleAsync(EventContext<WorkflowStartedEvent> context, CancellationToken cancellationToken)
    {
        await _publisher.PublishAsync(new WorkflowFinishedEvent(context.Payload.Result, typeof(GammaModule)), cancellationToken);
    }
}