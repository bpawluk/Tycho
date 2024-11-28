﻿using Tycho.Events;

namespace Tycho.IntegrationTests.ForwardingEventsHorizontally.SUT.Modules.Gamma.Handlers;

internal class GammaWorkflowStartedEventHandler(IEventPublisher publisher) : IEventHandler<GammaWorkflowStartedEvent>
{
    private readonly IEventPublisher _publisher = publisher;

    public async Task Handle(GammaWorkflowStartedEvent eventData, CancellationToken cancellationToken)
    {
        await _publisher.Publish(new GammaWorkflowFinishedEvent(eventData.Result), cancellationToken);
    }
}