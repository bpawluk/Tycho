using System.Threading;
using System.Threading.Tasks;
using TychoV2.Structure;

namespace TychoV2.Requests.Broker
{
    internal class UpStreamBroker : IExecute
    {
        private readonly Internals _internals;

        public UpStreamBroker(Internals internals)
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
