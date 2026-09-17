using System.Threading;
using System.Threading.Tasks;

namespace Tycho.Requests.Pipeline
{
    internal sealed class RequestPipeline<TRequest, TResponse>
        where TRequest : class
    {
        private RequestHandlerDelegate<TRequest, TResponse> _executeHandlerPipeline;

        public RequestPipeline(RequestHandlerDelegate<TRequest, TResponse> finalPipelineStep)
        {
            _executeHandlerPipeline = finalPipelineStep;
        }

        public void AddInterceptor(IRequestInterceptor<TRequest, TResponse> interceptor)
        {
            RequestHandlerDelegate<TRequest, TResponse> next = _executeHandlerPipeline;
            _executeHandlerPipeline = (requestData, cancellationToken) => interceptor.InterceptAsync(next, requestData, cancellationToken);
        }

        public Task<TResponse> ExecuteAsync(TRequest requestData, CancellationToken cancellationToken)
        {
            return _executeHandlerPipeline(requestData, cancellationToken);
        }
    }
}
