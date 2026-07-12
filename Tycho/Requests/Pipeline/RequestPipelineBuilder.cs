using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Transactions;

namespace Tycho.Requests.Pipeline
{
    internal static class RequestPipelineBuilder
    {
        public static RequestPipeline<TRequest, NoResponse> Build<TRequest>(IServiceProvider serviceProvider, IRequestHandler<TRequest> handler)
            where TRequest : class, IRequest
        {
            async Task<NoResponse> TerminalHandler(TRequest data, CancellationToken token)
            {
                await handler.HandleAsync(data, token).ConfigureAwait(false);
                return NoResponse.Value;
            }

            return Build<TRequest, NoResponse>(serviceProvider, TerminalHandler, handler is ITransactionalRequestHandler);
        }

        public static RequestPipeline<TRequest, TResponse> Build<TRequest, TResponse>(IServiceProvider serviceProvider, IRequestHandler<TRequest, TResponse> handler)
            where TRequest : class, IRequest<TResponse>
        {
            return Build<TRequest, TResponse>(serviceProvider, handler.HandleAsync, handler is ITransactionalRequestHandler);
        }

        private static RequestPipeline<TRequest, TResponse> Build<TRequest, TResponse>(IServiceProvider serviceProvider, RequestHandlerDelegate<TRequest, TResponse> terminalHandler, bool isTransactional)
            where TRequest : class
        {
            var requestPipeline = new RequestPipeline<TRequest, TResponse>(terminalHandler);

            if (isTransactional)
            {
                ITransaction transaction = serviceProvider.GetRequiredService<ITransaction>();
                var transactionalInterceptor = new TransactionInterceptor<TRequest, TResponse>(transaction);
                requestPipeline.AddInterceptor(transactionalInterceptor);
            }

            IEnumerable<IRequestInterceptor<TRequest, TResponse>> interceptors = serviceProvider.GetServices<IRequestInterceptor<TRequest, TResponse>>();
            foreach (IRequestInterceptor<TRequest, TResponse> interceptor in interceptors.Reverse())
            {
                requestPipeline.AddInterceptor(interceptor);
            }

            return requestPipeline;
        }
    }
}
