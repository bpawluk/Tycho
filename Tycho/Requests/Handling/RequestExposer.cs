using Tycho.Structure.Parent;

namespace Tycho.Requests.Handling
{
    internal class RequestExposer<TRequest> : RequestRelay<TRequest>
        where TRequest : class, IRequest
    {
        public RequestExposer(IParentReference parent) : base(parent.RequestBroker)
        {
        }
    }

    internal class RequestExposer<TRequest, TResponse> : RequestRelay<TRequest, TResponse>
        where TRequest : class, IRequest<TResponse>
    {
        public RequestExposer(IParentReference parent) : base(parent.RequestBroker)
        {
        }
    }
}
