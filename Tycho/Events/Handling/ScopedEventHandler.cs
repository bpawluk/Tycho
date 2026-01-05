using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Events.Inbox;
using Tycho.Structure.Internal;

namespace Tycho.Events.Handling
{
    internal class ScopedEventHandler<TEvent, THandler> : IEventHandler<TEvent>
        where TEvent : class, IEvent
        where THandler : IEventHandler<TEvent> 
    {
        private readonly Internals _internals;

        public ScopedEventHandler(Internals internals)
        {
            _internals = internals;
        }

        public async Task HandleAsync(EventContext<TEvent> context, CancellationToken cancellationToken)
        {
            await using var scope = _internals.CreateAsyncScope();

            var inbox = scope.ServiceProvider.GetRequiredService<IInboxConsumer>();
            var handler = scope.ServiceProvider.GetRequiredService<THandler>();

            var transactionalHandler = handler as ITransactionalEventHandler;
            if (transactionalHandler != null)
            {
                await transactionalHandler.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
            }

            try
            {
                await handler.HandleAsync(context, cancellationToken).ConfigureAwait(false);
                await inbox.MarkAsHandled(context.Id, cancellationToken).ConfigureAwait(false);

                if (transactionalHandler != null)
                {
                    await transactionalHandler.CommitTransactionAsync(cancellationToken).ConfigureAwait(false);
                }
            }
            catch
            {
                if (transactionalHandler != null)
                {
                    await transactionalHandler.RollbackTransactionAsync(cancellationToken).ConfigureAwait(false);
                }
                throw;
            }
        }
    }
}