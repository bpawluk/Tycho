using System.Threading;
using System.Threading.Tasks;

namespace NewTycho.Requests
{
    /// <summary>
    /// TODO
    /// </summary>
    public interface IExecute
    {
        Task Execute<TRequest>(TRequest requestData, CancellationToken cancellationToken = default)
            where TRequest : class, IRequest;

        Task<TResponse> Execute<TRequest, TResponse>(TRequest requestData, CancellationToken cancellationToken = default)
            where TRequest : class, IRequest<TResponse>;
    }
}
