using System.Threading;
using System.Threading.Tasks;
using Tycho.Events.Routing;
using Tycho.Requests;
using Tycho.Requests.Broker;
using Tycho.Structure;

namespace Tycho.Modules.Instance
{
    internal class Module<TModuleDefinition> : IModule<TModuleDefinition>
        where TModuleDefinition : TychoModule
    {
        private readonly IEventRouter _eventRouter;
        private readonly Internals _internals;
        private readonly UpStreamBroker _requestBroker;

        Internals IModule.Internals => _internals;

        IEventRouter IModule.EventRouter => _eventRouter;

        public Module(Internals internals)
        {
            _internals = internals;
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
            _internals.Dispose();
        }
    }
}