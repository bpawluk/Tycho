using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Requests
{
    /// <summary>
    /// TODO
    /// </summary>
    public interface IRequestExecutor
    {
        Task Execute<TRequest>(TRequest requestData, CancellationToken cancellationToken = default)
            where TRequest : class, IRequest;

        Task<TResponse> Execute<TRequest, TResponse>(TRequest requestData, CancellationToken cancellationToken = default)
            where TRequest : class, IRequest<TResponse>;
    }
}
