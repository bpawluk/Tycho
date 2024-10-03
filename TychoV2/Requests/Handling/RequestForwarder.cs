using System.Threading;
using System.Threading.Tasks;
using TychoV2.Modules;

namespace TychoV2.Requests.Handling
{
    internal class RequestForwarder<TRequest, TModule> : IHandle<TRequest>
        where TRequest : class, IRequest
        where TModule : TychoModule
    {
        private readonly IModule<TModule> _childModule;

        public RequestForwarder(IModule<TModule> childModule)
        {
            _childModule = childModule;
        }

        public Task Handle(TRequest requestData, CancellationToken cancellationToken) => 
            _childModule.Execute(requestData, cancellationToken);
    }

    internal class RequestForwarder<TRequest, TResponse, TModule> : IHandle<TRequest, TResponse>
        where TRequest : class, IRequest<TResponse>
        where TModule : TychoModule
    {
        private readonly IModule<TModule> _childModule;

        public RequestForwarder(IModule<TModule> childModule)
        {
            _childModule = childModule;
        }

        public Task<TResponse> Handle(TRequest requestData, CancellationToken cancellationToken) =>
            _childModule.Execute<TRequest, TResponse>(requestData, cancellationToken);
    }
}
