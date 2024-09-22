using System.Threading;
using System.Threading.Tasks;

namespace NewTycho.Requests
{
    /// <summary>
    /// TODO
    /// </summary>
    public interface IHandle<TRequest> : Messaging.IHandle<TRequest>
        where TRequest : class, IRequest 
    {
        Task Handle(TRequest requestData, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// TODO
    /// </summary>
    public interface IHandle<TRequest, TResponse> : Messaging.IHandle<TRequest>
        where TRequest : class, IRequest<TResponse> 
    {
        Task<TResponse> Handle(TRequest requestData, CancellationToken cancellationToken = default);
    }
}
