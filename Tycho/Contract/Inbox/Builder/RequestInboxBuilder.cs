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
            var handler = new LambdaWrappingRequestHandler<Request>(action);
            _moduleInbox.RegisterRequestHandler(handler);
            return this;
        }

        IInboxDefinition IRequestInboxDefinition.Handle<Request, Response>(Func<Request, Response> function)
        {
            var handler = new LambdaWrappingRequestHandler<Request, Response>(function);
            _moduleInbox.RegisterRequestWithResponseHandler(handler);
            return this;
        }

        IInboxDefinition IRequestInboxDefinition.Handle<Request>(Func<Request, Task> function)
        {
            var handler = new LambdaWrappingRequestHandler<Request>(function);
            _moduleInbox.RegisterRequestHandler(handler);
            return this;
        }

        IInboxDefinition IRequestInboxDefinition.Handle<Request, Response>(Func<Request, Task<Response>> function)
        {
            var handler = new LambdaWrappingRequestHandler<Request, Response>(function);
            _moduleInbox.RegisterRequestWithResponseHandler(handler);
            return this;
        }

        IInboxDefinition IRequestInboxDefinition.Handle<Request>(Func<Request, CancellationToken, Task> function)
        {
            var handler = new LambdaWrappingRequestHandler<Request>(function);
            _moduleInbox.RegisterRequestHandler(handler);
            return this;
        }

        IInboxDefinition IRequestInboxDefinition.Handle<Request, Response>(Func<Request, CancellationToken, Task<Response>> function)
        {
            var handler = new LambdaWrappingRequestHandler<Request, Response>(function);
            _moduleInbox.RegisterRequestWithResponseHandler(handler);
            return this;
        }

        IInboxDefinition IRequestInboxDefinition.Handle<Request>(IRequestHandler<Request> handler)
        {
            _moduleInbox.RegisterRequestHandler(handler);
            return this;
        }

        IInboxDefinition IRequestInboxDefinition.Handle<Request, Response>(IRequestHandler<Request, Response> handler)
        {
            _moduleInbox.RegisterRequestWithResponseHandler(handler);
            return this;
        }

        IInboxDefinition IRequestInboxDefinition.Handle<Request, Handler>()
        {
            Func<Handler> handlerCreator = () => _instanceCreator.CreateInstance<Handler>();
            var handler = new TransientRequestHandler<Request>(handlerCreator);
            _moduleInbox.RegisterRequestHandler(handler);
            return this;
        }

        IInboxDefinition IRequestInboxDefinition.Handle<Request, Response, Handler>()
        {
            Func<Handler> handlerCreator = () => _instanceCreator.CreateInstance<Handler>();
            var handler = new TransientRequestHandler<Request, Response>(handlerCreator);
            _moduleInbox.RegisterRequestWithResponseHandler(handler);
            return this;
        }

        IInboxDefinition IRequestInboxDefinition.Forward<Request, Module>()
        {
            Func<Request, Request> mapping = requestData => requestData;
            Func<RequestDownForwarder<Request, Request, Module>> forwarderCreator = () =>
            {
                return _instanceCreator.CreateInstance<RequestDownForwarder<Request, Request, Module>>(mapping);
            };
            var handler = new TransientRequestHandler<Request>(forwarderCreator);
            _moduleInbox.RegisterRequestHandler(handler);
            return this;
        }

        IInboxDefinition IRequestInboxDefinition.Forward<Request, Response, Module>()
        {
            Func<Request, Request> requestMapping = requestData => requestData;
            Func<Response, Response> responseMapping = response => response;
            Func<RequestDownForwarder<Request, Response, Request, Response, Module>> forwarderCreator = () =>
            {
                return _instanceCreator.CreateInstance<RequestDownForwarder<Request, Response, Request, Response, Module>>(requestMapping, responseMapping);
            };
            var handler = new TransientRequestHandler<Request, Response>(forwarderCreator);
            _moduleInbox.RegisterRequestWithResponseHandler(handler);
            return this;
        }

        IInboxDefinition IRequestInboxDefinition.Forward<RequestIn, RequestOut, Module>(Func<RequestIn, RequestOut> mapping)
        {
            Func<RequestDownForwarder<RequestIn, RequestOut, Module>> forwarderCreator = () =>
            {
                return _instanceCreator.CreateInstance<RequestDownForwarder<RequestIn, RequestOut, Module>>(mapping);
            };
            var handler = new TransientRequestHandler<RequestIn>(forwarderCreator);
            _moduleInbox.RegisterRequestHandler(handler);
            return this;
        }

        IInboxDefinition IRequestInboxDefinition.Forward<RequestIn, ResponseIn, RequestOut, ResponseOut, Module>(Func<RequestIn, RequestOut> requestMapping, Func<ResponseOut, ResponseIn> responseMapping)
        {
            Func<RequestDownForwarder<RequestIn, ResponseIn, RequestOut, ResponseOut, Module>> forwarderCreator = () =>
            {
                return _instanceCreator.CreateInstance<RequestDownForwarder<RequestIn, ResponseIn, RequestOut, ResponseOut, Module>>(requestMapping, responseMapping);
            };
            var handler = new TransientRequestHandler<RequestIn, ResponseIn>(forwarderCreator);
            _moduleInbox.RegisterRequestWithResponseHandler(handler);
            return this;
        }

        IInboxDefinition IRequestInboxDefinition.ForwardWithInterception<Request, Interceptor, Module>()
        {
            Func<Request, Request> mapping = requestData => requestData;
            Func<RequestDownForwarder<Request, Request, Module>> forwarderCreator = () =>
            {
                var interceptor = _instanceCreator.CreateInstance<Interceptor>();
                return _instanceCreator.CreateInstance<RequestDownForwarder<Request, Request, Module>>(mapping, interceptor);
            };
            var handler = new TransientRequestHandler<Request>(forwarderCreator);
            _moduleInbox.RegisterRequestHandler(handler);
            return this;
        }

        IInboxDefinition IRequestInboxDefinition.ForwardWithInterception<Request, Response, Interceptor, Module>()
        {
            Func<Request, Request> requestMapping = requestData => requestData;
            Func<Response, Response> responseMapping = response => response;
            Func<RequestDownForwarder<Request, Response, Request, Response, Module>> forwarderCreator = () =>
            {
                var interceptor = _instanceCreator.CreateInstance<Interceptor>();
                return _instanceCreator.CreateInstance<RequestDownForwarder<Request, Response, Request, Response, Module>>(requestMapping, responseMapping, interceptor);
            };
            var handler = new TransientRequestHandler<Request, Response>(forwarderCreator);
            _moduleInbox.RegisterRequestWithResponseHandler(handler);
            return this;
        }

        IInboxDefinition IRequestInboxDefinition.ForwardWithInterception<RequestIn, RequestOut, Interceptor, Module>(Func<RequestIn, RequestOut> mapping)
        {
            Func<RequestDownForwarder<RequestIn, RequestOut, Module>> forwarderCreator = () =>
            {
                var interceptor = _instanceCreator.CreateInstance<Interceptor>();
                return _instanceCreator.CreateInstance<RequestDownForwarder<RequestIn, RequestOut, Module>>(mapping, interceptor);
            };
            var handler = new TransientRequestHandler<RequestIn>(forwarderCreator);
            _moduleInbox.RegisterRequestHandler(handler);
            return this;
        }

        IInboxDefinition IRequestInboxDefinition.ForwardWithInterception<RequestIn, ResponseIn, RequestOut, ResponseOut, Interceptor, Module>(Func<RequestIn, RequestOut> requestMapping, Func<ResponseOut, ResponseIn> responseMapping)
        {
            Func<RequestDownForwarder<RequestIn, ResponseIn, RequestOut, ResponseOut, Module>> forwarderCreator = () =>
            {
                var interceptor = _instanceCreator.CreateInstance<Interceptor>();
                return _instanceCreator.CreateInstance<RequestDownForwarder<RequestIn, ResponseIn, RequestOut, ResponseOut, Module>>(requestMapping, responseMapping, interceptor);
            };
            var handler = new TransientRequestHandler<RequestIn, ResponseIn>(forwarderCreator);
            _moduleInbox.RegisterRequestWithResponseHandler(handler);
            return this;
        }
    }
}
