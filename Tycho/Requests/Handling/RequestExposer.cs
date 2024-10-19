using Tycho.Structure;

namespace Tycho.Requests.Handling
{
    internal class RequestExposer<TRequest> : RequestRelay<TRequest>
        where TRequest : class, IRequest
    {
        public RequestExposer(IParent parent) : base(parent)
        {
        }
    }

    internal class RequestExposer<TRequest, TResponse> : RequestRelay<TRequest, TResponse>
        where TRequest : class, IRequest<TResponse>
    {
        public RequestExposer(IParent parent) : base(parent)
        {
        }
    }
}