using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Structure;

namespace Tycho.Events.Handling
{
    internal class ScopedEventHandler<TEvent, TEventHandler> : IEventHandler<TEvent>
        where TEvent : class, IEvent
        where TEventHandler : IEventHandler<TEvent>
    {
        private readonly Internals _internals;

        public ScopedEventHandler(Internals internals)
        {
            _internals = internals;
        }

        public Task Handle(TEvent eventData, CancellationToken cancellationToken)
        {
            using var scope = _internals.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<TEventHandler>();
            return handler.Handle(eventData, cancellationToken);
        }
    }
}