using System.Threading;
using System.Threading.Tasks;
using Tycho.Modules;

namespace Tycho.Requests.Handling
{
    internal class RequestForwarder<TRequest, TModule> : IRequestHandler<TRequest>
        where TRequest : class, IRequest
        where TModule : TychoModule
    {
        private readonly IModule<TModule> _childModule;

        public RequestForwarder(IModule<TModule> childModule)
        {
            _childModule = childModule;
        }

        public Task Handle(TRequest requestData, CancellationToken cancellationToken)
        {
            return _childModule.Execute(requestData, cancellationToken);
        }
    }

    internal class RequestForwarder<TRequest, TResponse, TModule> : IRequestHandler<TRequest, TResponse>
        where TRequest : class, IRequest<TResponse>
        where TModule : TychoModule
    {
        private readonly IModule<TModule> _childModule;

        public RequestForwarder(IModule<TModule> childModule)
        {
            _childModule = childModule;
        }

        public Task<TResponse> Handle(TRequest requestData, CancellationToken cancellationToken)
        {
            return _childModule.Execute<TRequest, TResponse>(requestData, cancellationToken);
        }
    }
}