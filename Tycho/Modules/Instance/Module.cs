using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Events.Routing;
using Tycho.Requests;
using Tycho.Requests.Broker;
using Tycho.Structure;

namespace Tycho.Modules.Instance
{
    internal class Module<TModuleDefinition> : IModule<TModuleDefinition>
        where TModuleDefinition : TychoModule
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

        public void Dispose()
        {
            _cleanup(_internals);
            foreach (var module in _internals.GetServices<IModule>())
            {
                module.Dispose();
            }
            _internals.Dispose();
        }
    }
}