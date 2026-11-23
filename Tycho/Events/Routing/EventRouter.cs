using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Events.Model;
using Tycho.Structure;
using Tycho.Utils;

namespace Tycho.Events.Routing
{
    internal class EventRouter : IEventRouter
    {
        private readonly Internals _internals;

        public EventRouter(Internals internals)
        {
            _internals = internals;
        }

        [EntryPoint]
        public async Task<IReadOnlyCollection<RoutedEvent>> RouteAsync<TEvent>(
            Guid publishId,
            TEvent eventPayload,
            CancellationToken cancellationToken)
            where TEvent : class, IEvent
        {
            cancellationToken.ThrowIfCancellationRequested();

            AsyncServiceScope scope = _internals.CreateAsyncScope();
            await using ConfiguredAsyncDisposable scopeDisposal = scope.ConfigureAwait(false);

            IEventRouter scopedRouter = scope.ServiceProvider.GetRequiredService<IEventRouter>();
            return await scopedRouter.RouteAsync(publishId, eventPayload, cancellationToken).ConfigureAwait(false);
        }
    }
}
