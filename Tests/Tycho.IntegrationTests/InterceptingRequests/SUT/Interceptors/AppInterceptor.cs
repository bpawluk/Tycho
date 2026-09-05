using Tycho.IntegrationTests.InterceptingRequests.SUT.Utils;
using Tycho.Requests;

namespace Tycho.IntegrationTests.InterceptingRequests.SUT.Interceptors;

internal sealed class AppInterceptor<TRequest, TResponse> : IRequestInterceptor<TRequest, TResponse>
    where TRequest : class
{
    public async Task<TResponse> InterceptAsync(RequestHandlerDelegate<TRequest, TResponse> next, TRequest requestData, CancellationToken cancellationToken)
    {
        ITraceableRequest? traceableRequest = requestData as ITraceableRequest;
        traceableRequest?.Trace.Add("app-before");
        TResponse response = await next(requestData, cancellationToken);
        traceableRequest?.Trace.Add("app-after");
        return response;
    }
}
