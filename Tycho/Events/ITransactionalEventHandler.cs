using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Events
{
    /// <summary>
    /// Base interface for event handlers wrapping their logic in transactions.
    /// </summary>
    public interface ITransactionalEventHandler : IEventHandler
    {
        /// <summary>
        /// Begins a transaction that will wrap event handling logic.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token.</param>
        Task BeginTransactionAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Commits the transaction wrapping event handling logic.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token.</param>
        Task CommitTransactionAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Rolls back the transaction wrapping event handling logic.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token.</param>
        Task RollbackTransactionAsync(CancellationToken cancellationToken);
    }

    /// <summary>
    /// Transactional event handler for an event of type <typeparamref name="TEvent"/>.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event to handle.</typeparam>
    public interface ITransactionalEventHandler<TEvent> : ITransactionalEventHandler
        where TEvent : class, IEvent
    {
        /// <summary>
        /// Handles an event of type <typeparamref name="TEvent"/>.
        /// </summary>
        /// <param name="eventData">The data of the event to handle.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        Task Handle(TEvent eventData, CancellationToken cancellationToken);
    }
}
