using System.Threading;
using System.Threading.Tasks;
using TychoV2.Events.Routing;
using TychoV2.Requests;
using TychoV2.Requests.Broker;

namespace TychoV2.Structure
{
    internal class ParentProxy : IParent
    {
        private readonly IRequestBroker _contractFulfillingBroker;
        private readonly IEventRouter _parentEventRouter;

        IEventRouter IParent.EventRouter => _parentEventRouter;

        public ParentProxy(IRequestBroker contractFulfillingBroker, IEventRouter parentEventRouter)
        {
            _contractFulfillingBroker = contractFulfillingBroker;
            _parentEventRouter = parentEventRouter;
        }

        public Task Execute<TRequest>(TRequest requestData, CancellationToken cancellationToken)
            where TRequest : class, IRequest
        {
            return _contractFulfillingBroker.Execute(requestData, cancellationToken);
        }

        public Task<TResponse> Execute<TRequest, TResponse>(TRequest requestData, CancellationToken cancellationToken)
            where TRequest : class, IRequest<TResponse>
        {
            return _contractFulfillingBroker.Execute<TRequest, TResponse>(requestData, cancellationToken);
        }
    }
}
