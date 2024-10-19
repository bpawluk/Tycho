using Tycho.Modules;

namespace Tycho.Requests.Handling
{
    internal class RequestForwarder<TRequest, TModule> : RequestRelay<TRequest>
        where TRequest : class, IRequest
        where TModule : TychoModule
    {
        public RequestForwarder(IModule<TModule> childModule) : base(childModule)
        {
        }
    }

    internal class RequestForwarder<TRequest, TResponse, TModule> : RequestRelay<TRequest, TResponse>
        where TRequest : class, IRequest<TResponse>
        where TModule : TychoModule
    {
        public RequestForwarder(IModule<TModule> childModule) : base(childModule)
        {
        }
    }
}