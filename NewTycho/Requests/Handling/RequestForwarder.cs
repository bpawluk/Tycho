using NewTycho.Modules;
using System.Threading;
using System.Threading.Tasks;

namespace NewTycho.Requests.Handling
{
    internal class RequestForwarder<TRequest, TModule> : IHandle<TRequest>
        where TRequest : class, IRequest
        where TModule : TychoModule
    {
        public Task Handle(TRequest requestData, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }
    }

    internal class RequestForwarder<TRequest, TResponse, TModule> : IHandle<TRequest, TResponse>
        where TRequest : class, IRequest<TResponse>
        where TModule : TychoModule
    {
           public Task<TResponse> Handle(TRequest requestData, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }
}
