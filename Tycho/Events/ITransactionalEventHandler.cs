using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Events
{
    /// <summary>
    /// TBD
    /// </summary>
    public interface ITransactionalEventHandler : IEventHandler
    {
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);

        Task CommitTransactionAsync(CancellationToken cancellationToken = default);

        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// TBD
    /// </summary>
    public interface ITransactionalEventHandler<TEvent> : ITransactionalEventHandler
        where TEvent : class, IEvent
    {
        /// <summary>
        /// Handles an event of type <typeparamref name="TEvent"/>
        /// </summary>
        /// <param name="eventData">The data of the event to handle</param>
        Task Handle(TEvent eventData, CancellationToken cancellationToken);
    }
}
