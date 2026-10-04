using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Identity.Events;

namespace Tycho.Events.Model
{
    /// <summary>
    /// Represents an event ready for handling.
    /// </summary>
    public abstract class Event : EventBase
    {
        internal Event(Guid id, Guid publishId, EventIdentity eventId, EventHandlerIdentity handlerId) : base(id, publishId, eventId, handlerId)
        {
        }

        internal abstract IEventHandler GetHandlerFrom(IEventHandlerProvider provider);

        internal abstract Task HandleWith(IEventHandler handler, CancellationToken cancellationToken);
    }

    /// <summary>
    /// Represents an event ready for handling with a strongly typed payload.
    /// </summary>
    /// <typeparam name="TEvent">The event payload type.</typeparam>
    public class Event<TEvent> : Event where TEvent : class, IEvent
    {
        internal TEvent Payload { get; }

        internal Event(Guid id, Guid publishId, EventIdentity eventId, EventHandlerIdentity handlerId, TEvent payload) : base(id, publishId, eventId, handlerId)
        {
            Payload = payload;
        }

        internal override IEventHandler GetHandlerFrom(IEventHandlerProvider provider)
        {
            return provider.GetHandler<TEvent>(HandlerId);
        }

        internal override async Task HandleWith(IEventHandler handler, CancellationToken cancellationToken)
        {
            if (handler is IEventHandler<TEvent> typedHandler)
            {
                var context = new EventContext<TEvent>(Id, Payload);
                await typedHandler.HandleAsync(context, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                throw new ArgumentException($"Handler is not of type IEventHandler<{typeof(TEvent).Name}>");
            }
        }
    }
}
