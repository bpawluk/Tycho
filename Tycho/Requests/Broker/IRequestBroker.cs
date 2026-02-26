using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Requests.Broker
{
    internal interface IRequestBroker
    {
        bool CanExecute<TRequest>()
            where TRequest : class, IRequest;

        Task ExecuteAsync<TRequest>(TRequest requestData, CancellationToken cancellationToken = default)
            where TRequest : class, IRequest;

        bool CanExecute<TRequest, TResponse>()
            where TRequest : class, IRequest<TResponse>;

        Task<TResponse> ExecuteAsync<TRequest, TResponse>(TRequest requestData, CancellationToken cancellationToken = default)
            where TRequest : class, IRequest<TResponse>;
    }
}
