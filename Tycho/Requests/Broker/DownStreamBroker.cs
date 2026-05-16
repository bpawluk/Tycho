using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Modules;
using Tycho.Requests.Registrating.Registrations;
using Tycho.Structure;
using Tycho.Transactions;
using Tycho.Utils;

namespace Tycho.Requests.Broker
{
    internal class DownStreamBroker<TModule> : IRequestBroker
        where TModule : TychoModule
    {
        private readonly Internals _internals;

        public DownStreamBroker(Internals internals)
        {
            _internals = internals;
        }

        public bool CanExecute<TRequest>()
            where TRequest : class, IRequest
        {
            return _internals.HasService<IDownStreamRequestRegistration<TRequest, TModule>>();
        }

        public bool CanExecute<TRequest, TResponse>()
            where TRequest : class, IRequest<TResponse>
        {
            return _internals.HasService<IDownStreamRequestRegistration<TRequest, TResponse, TModule>>();
        }

        [EntryPoint]
        public async Task ExecuteAsync<TRequest>(TRequest requestData, CancellationToken cancellationToken)
            where TRequest : class, IRequest
        {
            requestData.ThrowIfNull();

            await using AsyncServiceScope scope = _internals.CreateAsyncScope();

            ITransaction transaction = scope.ServiceProvider.GetRequiredService<ITransaction>();
            IDownStreamRequestRegistration<TRequest, TModule> registration = scope.ServiceProvider.GetRequiredService<IDownStreamRequestRegistration<TRequest, TModule>>();

            if (registration.Handler is ITransactionalRequestHandler)
            {
                await transaction.BeginAsync(cancellationToken).ConfigureAwait(false);
            }

            try
            {
                await registration.Handler.HandleAsync(requestData, cancellationToken).ConfigureAwait(false);
                if (transaction.IsInProgress)
                {
                    await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
                }
            }
            catch
            {
                if (transaction.IsInProgress)
                {
                    await transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
                }
                throw;
            }
        }

        [EntryPoint]
        public async Task<TResponse> ExecuteAsync<TRequest, TResponse>(TRequest requestData, CancellationToken cancellationToken)
            where TRequest : class, IRequest<TResponse>
        {
            requestData.ThrowIfNull();

            await using AsyncServiceScope scope = _internals.CreateAsyncScope();

            ITransaction transaction = scope.ServiceProvider.GetRequiredService<ITransaction>();
            IDownStreamRequestRegistration<TRequest, TResponse, TModule> registration = scope.ServiceProvider.GetRequiredService<IDownStreamRequestRegistration<TRequest, TResponse, TModule>>();

            if (registration.Handler is ITransactionalRequestHandler)
            {
                await transaction.BeginAsync(cancellationToken).ConfigureAwait(false);
            }

            try
            {
                TResponse response = await registration.Handler.HandleAsync(requestData, cancellationToken).ConfigureAwait(false);
                if (transaction.IsInProgress)
                {
                    await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
                }
                return response;
            }
            catch
            {
                if (transaction.IsInProgress)
                {
                    await transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
                }
                throw;
            }
        }
    }
}
