using System.Threading;
using System.Threading.Tasks;
using TychoV2.Events.Routing;
using TychoV2.Requests;
using TychoV2.Requests.Broker;
using TychoV2.Structure;

namespace TychoV2.Modules.Instance
{
    internal class Module<TModuleDefinition> : IModule<TModuleDefinition>
        where TModuleDefinition : TychoModule
    {
        private readonly Internals _internals;
        private readonly UpStreamBroker _requestBroker;
        private readonly EventRouter _eventRouter;

        Internals IModule.Internals => _internals;

        EventRouter IModule.EventRouter => _eventRouter;

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
    }
}
