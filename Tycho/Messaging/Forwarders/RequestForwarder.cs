using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Handlers;
using Tycho.Messaging.Interceptors;
using Tycho.Messaging.Payload;

namespace Tycho.Messaging.Forwarders
{
    internal abstract class RequestForwarder<RequestIn, RequestOut>
        : ForwarderBase<RequestIn, RequestOut>
        , IRequestHandler<RequestIn>
        where RequestIn : class, IRequest
        where RequestOut : class, IRequest
    {
        private readonly IRequestInterceptor<RequestOut>? _interceptor;

        public RequestForwarder(
            IModule target,
            Func<RequestIn, RequestOut> mapping,
            IRequestInterceptor<RequestOut>? interceptor)
            : base(target, mapping)
        {
            _interceptor = interceptor;
        }

        public async Task Handle(RequestIn requestData, CancellationToken cancellationToken)
        {
            var mappedRequest = _messageMapping(requestData);

            if (_interceptor != null)
            {
                await _interceptor.ExecuteBefore(mappedRequest, cancellationToken).ConfigureAwait(false);
            }

            await _target.Execute(mappedRequest, cancellationToken).ConfigureAwait(false);

            if (_interceptor != null)
            {
                await _interceptor.ExecuteAfter(mappedRequest, cancellationToken).ConfigureAwait(false);
            }
        }
    }

    internal class RequestUpForwarder<RequestIn, RequestOut>
        : RequestForwarder<RequestIn, RequestOut>
        , IRequestHandler<RequestIn>
        where RequestIn : class, IRequest
        where RequestOut : class, IRequest
    {
        public RequestUpForwarder(
            IModule module,
            Func<RequestIn, RequestOut> mapping,
            IRequestInterceptor<RequestOut>? interceptor = null)
            : base(module, mapping, interceptor) { }
    }

    internal class RequestDownForwarder<RequestIn, RequestOut, Module>
        : RequestForwarder<RequestIn, RequestOut>
        , IRequestHandler<RequestIn>
        where RequestIn : class, IRequest
        where RequestOut : class, IRequest
        where Module : TychoModule
    {
        public RequestDownForwarder(
            IModule<Module> submodule,
            Func<RequestIn, RequestOut> mapping,
            IRequestInterceptor<RequestOut>? interceptor = null)
            : base(submodule, mapping, interceptor) { }
    } 
}
