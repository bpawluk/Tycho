using System.Threading;
using System.Threading.Tasks;

namespace TychoV2.Requests
{
    /// <summary>
    /// TODO
    /// </summary>
    public interface IHandle { }

    /// <summary>
    /// TODO
    /// </summary>
    public interface IHandle<TRequest> : IHandle
        where TRequest : class, IRequest 
    {
        Task Handle(TRequest requestData, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// TODO
    /// </summary>
    public interface IHandle<TRequest, TResponse> : IHandle
        where TRequest : class, IRequest<TResponse> 
    {
        Task<TResponse> Handle(TRequest requestData, CancellationToken cancellationToken = default);
    }
}
