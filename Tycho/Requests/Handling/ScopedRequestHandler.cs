using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Structure.Internal;

namespace Tycho.Requests.Handling
{
    internal class ScopedRequestHandler<TRequest, THandler> : IRequestHandler<TRequest>
        where TRequest : class, IRequest
        where THandler : IRequestHandler<TRequest>
    {
        private readonly Internals _internals;

        public ScopedRequestHandler(Internals internals)
        {
            _internals = internals;
        }

        public async Task Handle(TRequest requestData, CancellationToken cancellationToken)
        {
            await using var scope = _internals.CreateAsyncScope();
            var handler = scope.ServiceProvider.GetRequiredService<THandler>();

            var transactionalHandler = handler as ITransactionalRequestHandler;
            if (transactionalHandler != null)
            {
                await transactionalHandler.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
            }

            try
            {
                await handler.Handle(requestData, cancellationToken).ConfigureAwait(false);
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

    internal class ScopedRequestHandler<TRequest, TResponse, THandler> : IRequestHandler<TRequest, TResponse>
        where TRequest : class, IRequest<TResponse>
        where THandler : IRequestHandler<TRequest, TResponse>
    {
        private readonly Internals _internals;

        public ScopedRequestHandler(Internals internals)
        {
            _internals = internals;
        }

        public async Task<TResponse> Handle(TRequest requestData, CancellationToken cancellationToken)
        {
            await using var scope = _internals.CreateAsyncScope();
            var handler = scope.ServiceProvider.GetRequiredService<THandler>();

            var transactionalHandler = handler as ITransactionalRequestHandler;
            if (transactionalHandler != null)
            {
                await transactionalHandler.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
            }

            try
            {
                var result = await handler.Handle(requestData, cancellationToken).ConfigureAwait(false);
                if (transactionalHandler != null)
                {
                    await transactionalHandler.CommitTransactionAsync(cancellationToken).ConfigureAwait(false);
                }
                return result;
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