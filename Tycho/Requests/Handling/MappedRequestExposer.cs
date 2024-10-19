using System;
using Tycho.Structure;

namespace Tycho.Requests.Handling
{
    internal class MappedRequestExposer<TRequest, TTargetRequest> 
        : MappedRequestRelay<TRequest, TTargetRequest>
        where TRequest : class, IRequest
        where TTargetRequest : class, IRequest
    {
        public MappedRequestExposer(IParent parent, Func<TRequest, TTargetRequest> map)
            : base(parent, map)
        {
        }
    }

    internal class MappedRequestExposer<TRequest, TResponse, TTargetRequest, TTargetResponse> 
        : MappedRequestRelay<TRequest, TResponse, TTargetRequest, TTargetResponse>
        where TRequest : class, IRequest<TResponse>
        where TTargetRequest : class, IRequest<TTargetResponse>
    {
        public MappedRequestExposer(
            IParent parent, 
            Func<TRequest, TTargetRequest> mapRequest,
            Func<TTargetResponse, TResponse> mapResponse)
            : base(parent, mapRequest, mapResponse)
        {
        }
    }
}