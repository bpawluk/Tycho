using Tycho.IntegrationTests.InterceptingRequests.SUT.Utils;
using Tycho.Requests;

namespace Tycho.IntegrationTests.InterceptingRequests.SUT.Modules.Alpha.Interceptors;

internal sealed class ModuleInterceptor<TRequest, TResponse> : IRequestInterceptor<TRequest, TResponse>
    where TRequest : class
{
    public async Task<TResponse> InterceptAsync(RequestHandlerDelegate<TRequest, TResponse> next, TRequest requestData, CancellationToken cancellationToken)
    {
        ITraceableRequest? traceableRequest = requestData as ITraceableRequest;
        traceableRequest?.Trace.Add("module-before");
        TResponse response = await next(requestData, cancellationToken);
        traceableRequest?.Trace.Add("module-after");
        return response;
    }
}
