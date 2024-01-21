using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Forwarders;
using Tycho.Messaging.Handlers;
using Tycho.Messaging.Interceptors;
using Tycho.Messaging.Payload;

namespace Tycho.Contract.Inbox.Builder
{
    internal partial class InboxBuilder : IInboxDefinition
    {
        public IInboxDefinition RespondsTo<Query, Response>(Func<Query, Response> function)
            where Query : class, IQuery<Response>
        {
            var handler = new LambdaWrappingQueryHandler<Query, Response>(function);
            _moduleInbox.RegisterQueryHandler(handler);
            return this;
        }

        public IInboxDefinition RespondsTo<Query, Response>(Func<Query, Task<Response>> function)
            where Query : class, IQuery<Response>
        {
            var handler = new LambdaWrappingQueryHandler<Query, Response>(function);
            _moduleInbox.RegisterQueryHandler(handler);
            return this;
        }

        public IInboxDefinition RespondsTo<Query, Response>(Func<Query, CancellationToken, Task<Response>> function)
            where Query : class, IQuery<Response>
        {
            var handler = new LambdaWrappingQueryHandler<Query, Response>(function);
            _moduleInbox.RegisterQueryHandler(handler);
            return this;
        }

        public IInboxDefinition RespondsTo<Query, Response>(IQueryHandler<Query, Response> handler)
            where Query : class, IQuery<Response>
        {
            _moduleInbox.RegisterQueryHandler(handler);
            return this;
        }

        public IInboxDefinition RespondsTo<Query, Response, Handler>()
            where Handler : class, IQueryHandler<Query, Response>
            where Query : class, IQuery<Response>
        {
            Func<Handler> handlerCreator = () => _instanceCreator.CreateInstance<Handler>();
            var handler = new TransientQueryHandler<Query, Response>(handlerCreator);
            _moduleInbox.RegisterQueryHandler(handler);
            return this;
        }

        public IInboxDefinition ForwardsQuery<Query, Response, Module>()
            where Query : class, IQuery<Response>
            where Module : TychoModule
        {
            Func<Query, Query> queryMapping = queryData => queryData;
            Func<Response, Response> responseMapping = response => response;
            Func<QueryDownForwarder<Query, Response, Query, Response, Module>> forwarderCreator = () =>
            {
                return _instanceCreator.CreateInstance<QueryDownForwarder<Query, Response, Query, Response, Module>>(queryMapping, responseMapping);
            };
            var handler = new TransientQueryHandler<Query, Response>(forwarderCreator);
            _moduleInbox.RegisterQueryHandler(handler);
            return this;
        }

        public IInboxDefinition ForwardsQuery<Query, Response, Interceptor, Module>()
            where Query : class, IQuery<Response>
            where Interceptor : class, IQueryInterceptor<Query, Response>
            where Module : TychoModule
        {
            Func<Query, Query> queryMapping = queryData => queryData;
            Func<Response, Response> responseMapping = response => response;
            Func<QueryDownForwarder<Query, Response, Query, Response, Module>> forwarderCreator = () =>
            {
                var interceptor = _instanceCreator.CreateInstance<Interceptor>();
                return _instanceCreator.CreateInstance<QueryDownForwarder<Query, Response, Query, Response, Module>>(queryMapping, responseMapping, interceptor);
            };
            var handler = new TransientQueryHandler<Query, Response>(forwarderCreator);
            _moduleInbox.RegisterQueryHandler(handler);
            return this;
        }

        public IInboxDefinition ForwardsQuery<QueryIn, ResponseIn, QueryOut, ResponseOut, Module>(
            Func<QueryIn, QueryOut> queryMapping,
            Func<ResponseOut, ResponseIn> responseMapping)
            where QueryIn : class, IQuery<ResponseIn>
            where QueryOut : class, IQuery<ResponseOut>
            where Module : TychoModule
        {
            Func<QueryDownForwarder<QueryIn, ResponseIn, QueryOut, ResponseOut, Module>> forwarderCreator = () =>
            {
                return _instanceCreator.CreateInstance<QueryDownForwarder<QueryIn, ResponseIn, QueryOut, ResponseOut, Module>>(queryMapping, responseMapping);
            };
            var handler = new TransientQueryHandler<QueryIn, ResponseIn>(forwarderCreator);
            _moduleInbox.RegisterQueryHandler(handler);
            return this;
        }

        public IInboxDefinition ForwardsQuery<QueryIn, ResponseIn, QueryOut, ResponseOut, Interceptor, Module>(
            Func<QueryIn, QueryOut> queryMapping,
            Func<ResponseOut, ResponseIn> responseMapping)
            where QueryIn : class, IQuery<ResponseIn>
            where QueryOut : class, IQuery<ResponseOut>
            where Interceptor : class, IQueryInterceptor<QueryOut, ResponseOut>
            where Module : TychoModule
        {
            Func<QueryDownForwarder<QueryIn, ResponseIn, QueryOut, ResponseOut, Module>> forwarderCreator = () =>
            {
                var interceptor = _instanceCreator.CreateInstance<Interceptor>();
                return _instanceCreator.CreateInstance<QueryDownForwarder<QueryIn, ResponseIn, QueryOut, ResponseOut, Module>>(queryMapping, responseMapping, interceptor);
            };
            var handler = new TransientQueryHandler<QueryIn, ResponseIn>(forwarderCreator);
            _moduleInbox.RegisterQueryHandler(handler);
            return this;
        }
    }
}
