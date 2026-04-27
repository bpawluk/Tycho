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
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Commits the transaction wrapping event handling logic.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token.</param>
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Rolls back the transaction wrapping event handling logic.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token.</param>
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
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
