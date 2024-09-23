using System.Threading;
using System.Threading.Tasks;
using TychoV2.Modules;
using TychoV2.Structure;

namespace TychoV2.Requests.Broker
{
    internal class DownStreamBroker<TModule> : IExecute
        where TModule : TychoModule
    {
        private readonly Internals _internals;

        public DownStreamBroker(Internals internals)
        {
            _internals = internals;
        }

        public Task Execute<TRequest>(TRequest requestData, CancellationToken cancellationToken)
            where TRequest : class, IRequest
        {
            throw new System.NotImplementedException();
        }

        public Task<TResponse> Execute<TRequest, TResponse>(TRequest requestData, CancellationToken cancellationToken)
            where TRequest : class, IRequest<TResponse>
        {
            throw new System.NotImplementedException();
        }
    }
}
