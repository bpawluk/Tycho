using System;
using System.Threading;
using System.Threading.Tasks;
using Tycho.Messaging.Forwarders;
using Tycho.Messaging.Handlers;

namespace Tycho.Contract.Inbox.Builder
{
    internal partial class InboxBuilder : IInboxDefinition, IRequestInboxDefinition, IEventInboxDefinition
    {
        IInboxDefinition IRequestInboxDefinition.Handle<Request>(Action<Request> action)
        {
            var handler = new LambdaWrappingCommandHandler<Request>(action);
            _moduleInbox.RegisterCommandHandler(handler);
            return this;
        }

        IInboxDefinition IRequestInboxDefinition.Handle<Request, Response>(Func<Request, Response> function)
        {
            var handler = new LambdaWrappingQueryHandler<Request, Response>(function);
            _moduleInbox.RegisterQueryHandler(handler);
            return this;
        }

        IInboxDefinition IRequestInboxDefinition.Handle<Request>(Func<Request, Task> function)
        {
            var handler = new LambdaWrappingCommandHandler<Request>(function);
            _moduleInbox.RegisterCommandHandler(handler);
            return this;
        }

        IInboxDefinition IRequestInboxDefinition.Handle<Request, Response>(Func<Request, Task<Response>> function)
        {
            var handler = new LambdaWrappingQueryHandler<Request, Response>(function);
            _moduleInbox.RegisterQueryHandler(handler);
            return this;
        }

        IInboxDefinition IRequestInboxDefinition.Handle<Request>(Func<Request, CancellationToken, Task> function)
        {
            var handler = new LambdaWrappingCommandHandler<Request>(function);
            _moduleInbox.RegisterCommandHandler(handler);
            return this;
        }

        IInboxDefinition IRequestInboxDefinition.Handle<Request, Response>(Func<Request, CancellationToken, Task<Response>> function)
        {
            var handler = new LambdaWrappingQueryHandler<Request, Response>(function);
            _moduleInbox.RegisterQueryHandler(handler);
            return this;
        }

        IInboxDefinition IRequestInboxDefinition.Handle<Request>(ICommandHandler<Request> handler)
        {
            _moduleInbox.RegisterCommandHandler(handler);
            return this;
        }

        IInboxDefinition IRequestInboxDefinition.Handle<Request, Response>(IQueryHandler<Request, Response> handler)
        {
            _moduleInbox.RegisterQueryHandler(handler);
            return this;
        }

        IInboxDefinition IRequestInboxDefinition.Handle<Request, Handler>()
        {
            Func<Handler> handlerCreator = () => _instanceCreator.CreateInstance<Handler>();
            var handler = new TransientCommandHandler<Request>(handlerCreator);
            _moduleInbox.RegisterCommandHandler(handler);
            return this;
        }

        IInboxDefinition IRequestInboxDefinition.Handle<Request, Response, Handler>()
        {
            Func<Handler> handlerCreator = () => _instanceCreator.CreateInstance<Handler>();
            var handler = new TransientQueryHandler<Request, Response>(handlerCreator);
            _moduleInbox.RegisterQueryHandler(handler);
            return this;
        }

        IInboxDefinition IRequestInboxDefinition.Forward<Request, Module>()
        {
            Func<Request, Request> mapping = requestData => requestData;
            Func<CommandDownForwarder<Request, Request, Module>> forwarderCreator = () =>
            {
                return _instanceCreator.CreateInstance<CommandDownForwarder<Request, Request, Module>>(mapping);
            };
            var handler = new TransientCommandHandler<Request>(forwarderCreator);
            _moduleInbox.RegisterCommandHandler(handler);
            return this;
        }

        IInboxDefinition IRequestInboxDefinition.Forward<Request, Response, Module>()
        {
            Func<Request, Request> requestMapping = requestData => requestData;
            Func<Response, Response> responseMapping = response => response;
            Func<QueryDownForwarder<Request, Response, Request, Response, Module>> forwarderCreator = () =>
            {
                return _instanceCreator.CreateInstance<QueryDownForwarder<Request, Response, Request, Response, Module>>(requestMapping, responseMapping);
            };
            var handler = new TransientQueryHandler<Request, Response>(forwarderCreator);
            _moduleInbox.RegisterQueryHandler(handler);
            return this;
        }

        IInboxDefinition IRequestInboxDefinition.Forward<RequestIn, RequestOut, Module>(Func<RequestIn, RequestOut> mapping)
        {
            Func<CommandDownForwarder<RequestIn, RequestOut, Module>> forwarderCreator = () =>
            {
                return _instanceCreator.CreateInstance<CommandDownForwarder<RequestIn, RequestOut, Module>>(mapping);
            };
            var handler = new TransientCommandHandler<RequestIn>(forwarderCreator);
            _moduleInbox.RegisterCommandHandler(handler);
            return this;
        }

        IInboxDefinition IRequestInboxDefinition.Forward<RequestIn, ResponseIn, RequestOut, ResponseOut, Module>(Func<RequestIn, RequestOut> requestMapping, Func<ResponseOut, ResponseIn> responseMapping)
        {
            Func<QueryDownForwarder<RequestIn, ResponseIn, RequestOut, ResponseOut, Module>> forwarderCreator = () =>
            {
                return _instanceCreator.CreateInstance<QueryDownForwarder<RequestIn, ResponseIn, RequestOut, ResponseOut, Module>>(requestMapping, responseMapping);
            };
            var handler = new TransientQueryHandler<RequestIn, ResponseIn>(forwarderCreator);
            _moduleInbox.RegisterQueryHandler(handler);
            return this;
        }

        IInboxDefinition IRequestInboxDefinition.ForwardWithInterception<Request, Interceptor, Module>()
        {
            Func<Request, Request> mapping = requestData => requestData;
            Func<CommandDownForwarder<Request, Request, Module>> forwarderCreator = () =>
            {
                var interceptor = _instanceCreator.CreateInstance<Interceptor>();
                return _instanceCreator.CreateInstance<CommandDownForwarder<Request, Request, Module>>(mapping, interceptor);
            };
            var handler = new TransientCommandHandler<Request>(forwarderCreator);
            _moduleInbox.RegisterCommandHandler(handler);
            return this;
        }

        IInboxDefinition IRequestInboxDefinition.ForwardWithInterception<Request, Response, Interceptor, Module>()
        {
            Func<Request, Request> requestMapping = requestData => requestData;
            Func<Response, Response> responseMapping = response => response;
            Func<QueryDownForwarder<Request, Response, Request, Response, Module>> forwarderCreator = () =>
            {
                var interceptor = _instanceCreator.CreateInstance<Interceptor>();
                return _instanceCreator.CreateInstance<QueryDownForwarder<Request, Response, Request, Response, Module>>(requestMapping, responseMapping, interceptor);
            };
            var handler = new TransientQueryHandler<Request, Response>(forwarderCreator);
            _moduleInbox.RegisterQueryHandler(handler);
            return this;
        }

        IInboxDefinition IRequestInboxDefinition.ForwardWithInterception<RequestIn, RequestOut, Interceptor, Module>(Func<RequestIn, RequestOut> mapping)
        {
            Func<CommandDownForwarder<RequestIn, RequestOut, Module>> forwarderCreator = () =>
            {
                var interceptor = _instanceCreator.CreateInstance<Interceptor>();
                return _instanceCreator.CreateInstance<CommandDownForwarder<RequestIn, RequestOut, Module>>(mapping, interceptor);
            };
            var handler = new TransientCommandHandler<RequestIn>(forwarderCreator);
            _moduleInbox.RegisterCommandHandler(handler);
            return this;
        }

        IInboxDefinition IRequestInboxDefinition.ForwardWithInterception<RequestIn, ResponseIn, RequestOut, ResponseOut, Interceptor, Module>(Func<RequestIn, RequestOut> requestMapping, Func<ResponseOut, ResponseIn> responseMapping)
        {
            Func<QueryDownForwarder<RequestIn, ResponseIn, RequestOut, ResponseOut, Module>> forwarderCreator = () =>
            {
                var interceptor = _instanceCreator.CreateInstance<Interceptor>();
                return _instanceCreator.CreateInstance<QueryDownForwarder<RequestIn, ResponseIn, RequestOut, ResponseOut, Module>>(requestMapping, responseMapping, interceptor);
            };
            var handler = new TransientQueryHandler<RequestIn, ResponseIn>(forwarderCreator);
            _moduleInbox.RegisterQueryHandler(handler);
            return this;
        }
    }
}
