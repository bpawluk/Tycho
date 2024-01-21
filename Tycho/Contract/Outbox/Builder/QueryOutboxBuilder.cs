using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Forwarders;
using Tycho.Messaging.Handlers;
using Tycho.Messaging.Interceptors;
using Tycho.Messaging.Payload;

namespace Tycho.Contract.Outbox.Builder
{
    internal partial class OutboxBuilder : IOutboxDefinition, IOutboxConsumer
    {
        public IOutboxDefinition Sends<Query, Response>()
            where Query : class, IQuery<Response>
        {
            AddMessageDefinition(typeof(Query), nameof(Query));
            return this;
        }

        public IOutboxConsumer HandleQuery<Query, Response>(Func<Query, Response> function)
            where Query : class, IQuery<Response>
        {
            var handler = new LambdaWrappingQueryHandler<Query, Response>(function);
            RegisterQueryHandler(handler);
            return this;
        }

        public IOutboxConsumer HandleQuery<Query, Response>(Func<Query, Task<Response>> function)
            where Query : class, IQuery<Response>
        {
            var handler = new LambdaWrappingQueryHandler<Query, Response>(function);
            RegisterQueryHandler(handler);
            return this;
        }

        public IOutboxConsumer HandleQuery<Query, Response>(Func<Query, CancellationToken, Task<Response>> function)
            where Query : class, IQuery<Response>
        {
            var handler = new LambdaWrappingQueryHandler<Query, Response>(function);
            RegisterQueryHandler(handler);
            return this;
        }

        public IOutboxConsumer HandleQuery<Query, Response>(IQueryHandler<Query, Response> handler)
            where Query : class, IQuery<Response>
        {
            RegisterQueryHandler(handler);
            return this;
        }

        public IOutboxConsumer HandleQuery<Query, Response, Handler>()
            where Handler : class, IQueryHandler<Query, Response>
            where Query : class, IQuery<Response>
        {
            Func<Handler> handlerCreator = () => _instanceCreator.CreateInstance<Handler>();
            var handler = new TransientQueryHandler<Query, Response>(handlerCreator);
            RegisterQueryHandler(handler);
            return this;
        }

        public IOutboxConsumer ForwardQuery<Query, Response, Module>()
            where Query : class, IQuery<Response>
            where Module : TychoModule
        {
            Func<Query, Query> queryMapping = queryData => queryData;
            Func<Response, Response> responseMapping = response => response;
            Func<IQueryHandler<Query, Response>> handlerCreator = () =>
            {
                return _instanceCreator.CreateInstance<QueryDownForwarder<Query, Response, Query, Response, Module>>(queryMapping, responseMapping);
            };
            var handler = new TransientQueryHandler<Query, Response>(handlerCreator);
            RegisterQueryHandler(handler);
            return this;
        }

        public IOutboxConsumer ForwardQuery<Query, Response, Interceptor, Module>()
            where Query : class, IQuery<Response>
            where Interceptor : class, IQueryInterceptor<Query, Response>
            where Module : TychoModule
        {
            Func<Query, Query> queryMapping = queryData => queryData;
            Func<Response, Response> responseMapping = response => response;
            Func<IQueryHandler<Query, Response>> handlerCreator = () =>
            {
                var interceptor = _instanceCreator.CreateInstance<Interceptor>();
                return _instanceCreator.CreateInstance<QueryDownForwarder<Query, Response, Query, Response, Module>>(queryMapping, responseMapping, interceptor);
            };
            var handler = new TransientQueryHandler<Query, Response>(handlerCreator);
            RegisterQueryHandler(handler);
            return this;
        }

        public IOutboxConsumer ForwardQuery<QueryIn, ResponseIn, QueryOut, ResponseOut, Module>(
            Func<QueryIn, QueryOut> queryMapping,
            Func<ResponseOut, ResponseIn> responseMapping)
            where QueryIn : class, IQuery<ResponseIn>
            where QueryOut : class, IQuery<ResponseOut>
            where Module : TychoModule
        {
            Func<IQueryHandler<QueryIn, ResponseIn>> handlerCreator = () => 
            { 
                return _instanceCreator.CreateInstance<QueryDownForwarder<QueryIn, ResponseIn, QueryOut, ResponseOut, Module>>(queryMapping, responseMapping); 
            };
            var handler = new TransientQueryHandler<QueryIn, ResponseIn>(handlerCreator);
            RegisterQueryHandler(handler);
            return this;
        }

        public IOutboxConsumer ForwardQuery<QueryIn, ResponseIn, QueryOut, ResponseOut, Interceptor, Module>(
            Func<QueryIn, QueryOut> queryMapping,
            Func<ResponseOut, ResponseIn> responseMapping)
            where QueryIn : class, IQuery<ResponseIn>
            where QueryOut : class, IQuery<ResponseOut>
            where Interceptor : class, IQueryInterceptor<QueryOut, ResponseOut>
            where Module : TychoModule
        {
            Func<IQueryHandler<QueryIn, ResponseIn>> handlerCreator = () =>
            {
                var interceptor = _instanceCreator.CreateInstance<Interceptor>();
                return _instanceCreator.CreateInstance<QueryDownForwarder<QueryIn, ResponseIn, QueryOut, ResponseOut, Module>>(queryMapping, responseMapping, interceptor);
            };
            var handler = new TransientQueryHandler<QueryIn, ResponseIn>(handlerCreator);
            RegisterQueryHandler(handler);
            return this;
        }

        public IOutboxConsumer ExposeQuery<Query, Response>()
            where Query : class, IQuery<Response>
        {
            Func<Query, Query> queryMapping = queryData => queryData;
            Func<Response, Response> responseMapping = response => response;
            Func<IQueryHandler<Query, Response>> handlerCreator = () =>
            {
                return _instanceCreator.CreateInstance<QueryUpForwarder<Query, Response, Query, Response>>(queryMapping, responseMapping);
            };
            var handler = new TransientQueryHandler<Query, Response>(handlerCreator);
            RegisterQueryHandler(handler);
            return this;
        }

        public IOutboxConsumer ExposeQuery<Query, Response, Interceptor>()
            where Query : class, IQuery<Response>
            where Interceptor : class, IQueryInterceptor<Query, Response>
        {
            Func<Query, Query> queryMapping = queryData => queryData;
            Func<Response, Response> responseMapping = response => response;
            Func<IQueryHandler<Query, Response>> handlerCreator = () =>
            {
                var interceptor = _instanceCreator.CreateInstance<Interceptor>();
                return _instanceCreator.CreateInstance<QueryUpForwarder<Query, Response, Query, Response>>(queryMapping, responseMapping, interceptor);
            };
            var handler = new TransientQueryHandler<Query, Response>(handlerCreator);
            RegisterQueryHandler(handler);
            return this;
        }

        public IOutboxConsumer ExposeQuery<QueryIn, ResponseIn, QueryOut, ResponseOut>(
            Func<QueryIn, QueryOut> queryMapping,
            Func<ResponseOut, ResponseIn> responseMapping)
            where QueryIn : class, IQuery<ResponseIn>
            where QueryOut : class, IQuery<ResponseOut>
        {
            Func<IQueryHandler<QueryIn, ResponseIn>> handlerCreator = () =>
            {
                return _instanceCreator.CreateInstance<QueryUpForwarder<QueryIn, ResponseIn, QueryOut, ResponseOut>>(queryMapping, responseMapping);
            };
            var handler = new TransientQueryHandler<QueryIn, ResponseIn>(handlerCreator);
            RegisterQueryHandler(handler);
            return this;
        }

        public IOutboxConsumer ExposeQuery<QueryIn, ResponseIn, QueryOut, ResponseOut, Interceptor>(
            Func<QueryIn, QueryOut> queryMapping,
            Func<ResponseOut, ResponseIn> responseMapping)
            where QueryIn : class, IQuery<ResponseIn>
            where QueryOut : class, IQuery<ResponseOut>
            where Interceptor : class, IQueryInterceptor<QueryOut, ResponseOut>
        {
            Func<IQueryHandler<QueryIn, ResponseIn>> handlerCreator = () =>
            {
                var interceptor = _instanceCreator.CreateInstance<Interceptor>();
                return _instanceCreator.CreateInstance<QueryUpForwarder<QueryIn, ResponseIn, QueryOut, ResponseOut>>(queryMapping, responseMapping, interceptor);
            };
            var handler = new TransientQueryHandler<QueryIn, ResponseIn>(handlerCreator);
            RegisterQueryHandler(handler);
            return this;
        }

        public IOutboxConsumer IgnoreQuery<Query, Response>(Response response)
            where Query : class, IQuery<Response>
        {
            var handler = new StubQueryHandler<Query, Response>(response);
            RegisterQueryHandler(handler);
            return this;
        }

        public IOutboxConsumer RegisterQueryHandler<Query, Response>(IQueryHandler<Query, Response> handler)
            where Query : class, IQuery<Response>
        {
            ValidateIfMessageIsDefined(typeof(Query), nameof(Query));
            _moduleInbox.RegisterQueryHandler(handler);
            MarkMessageAsHandled(typeof(Query));
            return this;
        }
    }
}
