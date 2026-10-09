using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Model;
using Tycho.Events.Outbox;
using Tycho.Events.Routing;
using Tycho.Utils;

namespace Tycho.Events.Publishing
{
    internal class EventPublisher : IEventPublisher
    {
        private readonly IEventRouter _router;
        private readonly IOutboxWriter _outbox;

        public EventPublisher(IEventRouter router, IOutboxWriter outbox)
        {
            _router = router;
            _outbox = outbox;
        }

        async Task IEventPublisher.PublishAsync<TEvent>(TEvent eventPayload, CancellationToken cancellationToken)
        {
            eventPayload.ThrowIfNull();
            var publishId = Guid.NewGuid();
            IReadOnlyCollection<RoutedEvent> routedEvents = await _router.RouteAsync(publishId, eventPayload, cancellationToken).ConfigureAwait(false);
            if (routedEvents != null && routedEvents.Count > 0)
            {
                await _outbox.Write(routedEvents, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
