using System.Threading;
using System.Threading.Tasks;
using TychoV2.Requests;
using TychoV2.Requests.Broker;

namespace TychoV2.Structure
{
    internal class ParentProxy : IParent
    {
        private readonly IRequestBroker _contractFulfillingBroker;

        public ParentProxy(IRequestBroker contractFulfillingBroker)
        {
            _contractFulfillingBroker = contractFulfillingBroker;
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
