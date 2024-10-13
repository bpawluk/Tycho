using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Requests
{
    /// <summary>
    /// TODO
    /// </summary>
    public interface IRequestHandler { }

    /// <summary>
    /// TODO
    /// </summary>
    public interface IRequestHandler<TRequest> : IRequestHandler
        where TRequest : class, IRequest 
    {
        Task Handle(TRequest requestData, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// TODO
    /// </summary>
    public interface IRequestHandler<TRequest, TResponse> : IRequestHandler
        where TRequest : class, IRequest<TResponse> 
    {
        Task<TResponse> Handle(TRequest requestData, CancellationToken cancellationToken = default);
    }
}
