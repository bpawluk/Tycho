using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Requests;
using Tycho.Requests.Broker;
using Tycho.Structure;
using Tycho.Structure.Data;

namespace Tycho.Apps.Instance
{
    internal class App<TAppDefinition> : IApp<TAppDefinition>
        where TAppDefinition : TychoApp
    {
        private readonly Internals _internals;
        private readonly Func<IServiceProvider, Task> _cleanup;

        private readonly UpStreamBroker _requestBroker;

        Internals IApp.Internals => _internals;

        public App(Internals internals, Func<IServiceProvider, Task> cleanup)
        {
            _internals = internals;
            _cleanup = cleanup;
            _requestBroker = new UpStreamBroker(_internals);
        }

        public Task Execute<TRequest>(TRequest requestData, CancellationToken cancellationToken)
            where TRequest : class, IRequest
        {
            return _requestBroker.Execute(requestData, cancellationToken);
        }

        public Task<TResponse> Execute<TRequest, TResponse>(TRequest requestData, CancellationToken cancellationToken)
            where TRequest : class, IRequest<TResponse>
        {
            return _requestBroker.Execute<TRequest, TResponse>(requestData, cancellationToken);
        }

        public async ValueTask DisposeAsync()
        {
            await _cleanup(_internals).ConfigureAwait(false);
            foreach (var module in _internals.GetServices<IModule>())
            {
                await module.DisposeAsync().ConfigureAwait(false);
            }
            _internals.Dispose();
        }
    }
}