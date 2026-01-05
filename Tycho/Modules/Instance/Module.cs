using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Events.Routing;
using Tycho.Registry;
using Tycho.Requests;
using Tycho.Requests.Broker;
using Tycho.Structure;
using Tycho.Structure.Internal;
using Tycho.Utils;

namespace Tycho.Modules.Instance
{
    internal class Module<TTychoDefinition> : IModule<TTychoDefinition>
        where TTychoDefinition : TychoModule
    {
        private readonly Internals _internals;
        private readonly Func<IServiceProvider, Task> _cleanup;

        private readonly UpStreamBroker _requestBroker;
        private readonly IEventRouter _eventRouter;

        Internals IModule.Internals => _internals;
        IEventRouter IModule.EventRouter => _eventRouter;

        public Module(Internals internals, Func<IServiceProvider, Task> cleanup)
        {
            _internals = internals;
            _cleanup = cleanup;
            _requestBroker = new UpStreamBroker(_internals);
            _eventRouter = new EventRouter(_internals);
        }

        public Task ExecuteAsync<TRequest>(TRequest requestData, CancellationToken cancellationToken)
            where TRequest : class, IRequest
        {
            requestData.ThrowIfNull(nameof(requestData));
            return _requestBroker.ExecuteAsync(requestData, cancellationToken);
        }

        public Task<TResponse> ExecuteAsync<TRequest, TResponse>(TRequest requestData, CancellationToken cancellationToken)
            where TRequest : class, IRequest<TResponse>
        {
            requestData.ThrowIfNull(nameof(requestData));
            return _requestBroker.ExecuteAsync<TRequest, TResponse>(requestData, cancellationToken);
        }

        public async ValueTask DisposeAsync()
        {
            var moduleRegistry = _internals.GetRequiredService<IModuleRegistry>();

            try
            {
                await _cleanup(_internals).ConfigureAwait(false);
            }
            catch { }

            foreach (var module in moduleRegistry.GetAllModules())
            {
                try
                {
                    await module.DisposeAsync().ConfigureAwait(false);
                }
                catch { }
            }

            _internals.Dispose();
        }
    }
}
