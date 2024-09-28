using System.Threading;
using System.Threading.Tasks;
using TychoV2.Modules;

namespace TychoV2.Requests.Handling
{
    internal class RequestExposer<TRequest> : IHandle<TRequest>
        where TRequest : class, IRequest
    {
        private readonly IModule _thisModule;

        public RequestExposer(IModule thisModule)
        {
            _thisModule = thisModule;
        }

        public Task Handle(TRequest requestData, CancellationToken cancellationToken) => _thisModule.Execute(requestData, cancellationToken);
    }

    internal class RequestExposer<TRequest, TResponse> : IHandle<TRequest, TResponse>
        where TRequest : class, IRequest<TResponse>
    {
        private readonly IModule _thisModule;

        public RequestExposer(IModule thisModule)
        {
            _thisModule = thisModule;
        }

        public Task<TResponse> Handle(TRequest requestData, CancellationToken cancellationToken) => _thisModule.Execute<TRequest, TResponse>(requestData, cancellationToken);
    }
}
