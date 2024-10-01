using System;
using System.Threading;
using System.Threading.Tasks;
using TychoV2.Requests;
using TychoV2.Requests.Broker;
using TychoV2.Structure;

namespace TychoV2.Modules.Instance
{
    internal class ParentProxy : IModule
    {
        private readonly IRequestBroker _contractFulfillingBroker;

        Internals IModule.Internals => throw new InvalidOperationException();

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
