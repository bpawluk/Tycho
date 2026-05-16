using Tycho.Events;

namespace Tycho.Transactions
{
    /// <summary>
    /// Base interface for event handlers that support transactional behavior.
    /// </summary>
    public interface ITransactionalEventHandler : IEventHandler
    {
    }

    /// <summary>
    /// Transactional event handler for an event of type <typeparamref name="TEvent"/>.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event to handle.</typeparam>
    public interface ITransactionalEventHandler<TEvent> : ITransactionalEventHandler, IEventHandler<TEvent>
        where TEvent : class, IEvent
    {
    }
}
