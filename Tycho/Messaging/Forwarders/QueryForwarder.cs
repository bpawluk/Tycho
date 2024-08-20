using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Handlers;
using Tycho.Messaging.Interceptors;
using Tycho.Messaging.Payload;

namespace Tycho.Messaging.Forwarders
{

    internal abstract class QueryForwarder<QueryIn, ResponseIn, QueryOut, ResponseOut>
        : ForwarderBase<QueryIn, QueryOut>
        , IQueryHandler<QueryIn, ResponseIn>
        where QueryIn : class, IRequest<ResponseIn>
        where QueryOut : class, IRequest<ResponseOut>
    {
        private readonly Func<ResponseOut, ResponseIn> _responseMapping;
        private readonly IQueryInterceptor<QueryOut, ResponseOut>? _interceptor;

        public QueryForwarder(
            IModule target,
            Func<QueryIn, QueryOut> queryMapping,
            Func<ResponseOut, ResponseIn> responseMapping,
            IQueryInterceptor<QueryOut, ResponseOut>? interceptor)
            : base(target, queryMapping)
        {
            _responseMapping = responseMapping;
            _interceptor = interceptor;
        }

        public async Task<ResponseIn> Handle(QueryIn queryData, CancellationToken cancellationToken)
        {
            var mappedQuery = _messageMapping(queryData);

            if (_interceptor != null)
            {
                await _interceptor.ExecuteBefore(mappedQuery, cancellationToken).ConfigureAwait(false);
            }

            var response = await _target.Execute<QueryOut, ResponseOut>(mappedQuery, cancellationToken).ConfigureAwait(false);

            if (_interceptor != null)
            {
                response = await _interceptor.ExecuteAfter(mappedQuery, response, cancellationToken).ConfigureAwait(false);
            }

            return _responseMapping(response);
        }
    }

    internal class QueryUpForwarder<QueryIn, ResponseIn, QueryOut, ResponseOut>
        : QueryForwarder<QueryIn, ResponseIn, QueryOut, ResponseOut>
        , IQueryHandler<QueryIn, ResponseIn>
        where QueryIn : class, IRequest<ResponseIn>
        where QueryOut : class, IRequest<ResponseOut>
    {
        public QueryUpForwarder(
            IModule module,
            Func<QueryIn, QueryOut> queryMapping,
            Func<ResponseOut, ResponseIn> responseMapping,
            IQueryInterceptor<QueryOut, ResponseOut>? interceptor = null)
            : base(module, queryMapping, responseMapping, interceptor) { }
    }

    internal class QueryDownForwarder<QueryIn, ResponseIn, QueryOut, ResponseOut, Module>
        : QueryForwarder<QueryIn, ResponseIn, QueryOut, ResponseOut>
        , IQueryHandler<QueryIn, ResponseIn>
        where QueryIn : class, IRequest<ResponseIn>
        where QueryOut : class, IRequest<ResponseOut>
        where Module : TychoModule
    {
        public QueryDownForwarder(
            IModule<Module> submodule,
            Func<QueryIn, QueryOut> queryMapping,
            Func<ResponseOut, ResponseIn> responseMapping,
            IQueryInterceptor<QueryOut, ResponseOut>? interceptor = null)
            : base(submodule, queryMapping, responseMapping, interceptor) { }
    }
}
