using System;
using Tycho.Modules;
using Tycho.Structure;

namespace Tycho.Requests.Handling
{
    internal class MappedRequestForwarder<TRequest, TTargetRequest, TModule> 
        : MappedRequestRelay<TRequest, TTargetRequest>
        where TRequest : class, IRequest
        where TTargetRequest : class, IRequest
        where TModule : TychoModule
    {
        public MappedRequestForwarder(IModule<TModule> childModule, Func<TRequest, TTargetRequest> map)
            : base(childModule, map)
        {
        }
    }

    internal class MappedRequestForwarder<TRequest, TResponse, TTargetRequest, TTargetResponse, TModule> 
        : MappedRequestRelay<TRequest, TResponse, TTargetRequest, TTargetResponse>
        where TRequest : class, IRequest<TResponse>
        where TTargetRequest : class, IRequest<TTargetResponse>
        where TModule : TychoModule
    {
        public MappedRequestForwarder(
            IModule<TModule> childModule,
            Func<TRequest, TTargetRequest> mapRequest,
            Func<TTargetResponse, TResponse> mapResponse)
            : base(childModule, mapRequest, mapResponse)
        {
        }
    }
}