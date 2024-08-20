using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Handlers;
using Tycho.Messaging.Interceptors;
using Tycho.Messaging.Payload;

namespace Tycho.Messaging.Forwarders
{
    internal abstract class RequestForwarder<RequestIn, ResponseIn, RequestOut, ResponseOut>
        : ForwarderBase<RequestIn, RequestOut>
        , IRequestHandler<RequestIn, ResponseIn>
        where RequestIn : class, IRequest<ResponseIn>
        where RequestOut : class, IRequest<ResponseOut>
    {
        private readonly Func<ResponseOut, ResponseIn> _responseMapping;
        private readonly IRequestInterceptor<RequestOut, ResponseOut>? _interceptor;

        public RequestForwarder(
            IModule target,
            Func<RequestIn, RequestOut> requestMapping,
            Func<ResponseOut, ResponseIn> responseMapping,
            IRequestInterceptor<RequestOut, ResponseOut>? interceptor)
            : base(target, requestMapping)
        {
            _responseMapping = responseMapping;
            _interceptor = interceptor;
        }

        public async Task<ResponseIn> Handle(RequestIn requestData, CancellationToken cancellationToken)
        {
            var mappedRequest = _messageMapping(requestData);

            if (_interceptor != null)
            {
                await _interceptor.ExecuteBefore(mappedRequest, cancellationToken).ConfigureAwait(false);
            }

            var response = await _target.Execute<RequestOut, ResponseOut>(mappedRequest, cancellationToken).ConfigureAwait(false);

            if (_interceptor != null)
            {
                response = await _interceptor.ExecuteAfter(mappedRequest, response, cancellationToken).ConfigureAwait(false);
            }

            return _responseMapping(response);
        }
    }

    internal class RequestUpForwarder<RequestIn, ResponseIn, RequestOut, ResponseOut>
        : RequestForwarder<RequestIn, ResponseIn, RequestOut, ResponseOut>
        , IRequestHandler<RequestIn, ResponseIn>
        where RequestIn : class, IRequest<ResponseIn>
        where RequestOut : class, IRequest<ResponseOut>
    {
        public RequestUpForwarder(
            IModule module,
            Func<RequestIn, RequestOut> requestMapping,
            Func<ResponseOut, ResponseIn> responseMapping,
            IRequestInterceptor<RequestOut, ResponseOut>? interceptor = null)
            : base(module, requestMapping, responseMapping, interceptor) { }
    }

    internal class RequestDownForwarder<RequestIn, ResponseIn, RequestOut, ResponseOut, Module>
        : RequestForwarder<RequestIn, ResponseIn, RequestOut, ResponseOut>
        , IRequestHandler<RequestIn, ResponseIn>
        where RequestIn : class, IRequest<ResponseIn>
        where RequestOut : class, IRequest<ResponseOut>
        where Module : TychoModule
    {
        public RequestDownForwarder(
            IModule<Module> submodule,
            Func<RequestIn, RequestOut> requestMapping,
            Func<ResponseOut, ResponseIn> responseMapping,
            IRequestInterceptor<RequestOut, ResponseOut>? interceptor = null)
            : base(submodule, requestMapping, responseMapping, interceptor) { }
    }
}
