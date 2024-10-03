using System.Threading;
using System.Threading.Tasks;
using TychoV2.Structure;

namespace TychoV2.Requests.Handling
{
    internal class RequestExposer<TRequest> : IHandle<TRequest>
        where TRequest : class, IRequest
    {
        private readonly IParent _parent;

        public RequestExposer(IParent parent)
        {
            _parent = parent;
        }

        public Task Handle(TRequest requestData, CancellationToken cancellationToken) => 
            _parent.Execute(requestData, cancellationToken);
    }

    internal class RequestExposer<TRequest, TResponse> : IHandle<TRequest, TResponse>
        where TRequest : class, IRequest<TResponse>
    {
        private readonly IParent _parent;

        public RequestExposer(IParent parent)
        {
            _parent = parent;
        }

        public Task<TResponse> Handle(TRequest requestData, CancellationToken cancellationToken) =>
            _parent.Execute<TRequest, TResponse>(requestData, cancellationToken);
    }
}
