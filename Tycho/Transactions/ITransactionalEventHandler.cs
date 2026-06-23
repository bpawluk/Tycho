using Tycho.Events;

namespace Tycho.Transactions
{
    /// <summary>
    /// Base interface for Event Handlers that support transactional behavior.
    /// </summary>
    public interface ITransactionalEventHandler : IEventHandler
    {
    }

    /// <summary>
    /// Transactional Event Handler for an Event of type <typeparamref name="TEvent"/>.
    /// </summary>
    /// <typeparam name="TEvent">The type of the Event to handle.</typeparam>
    public interface ITransactionalEventHandler<TEvent> : ITransactionalEventHandler, IEventHandler<TEvent>
        where TEvent : class, IEvent
    {
    }
}
