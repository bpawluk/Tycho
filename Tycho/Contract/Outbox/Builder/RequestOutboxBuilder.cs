using System;
using Tycho.Messaging.Forwarders;
using Tycho.Messaging.Handlers;
using Tycho.Messaging.Payload;

namespace Tycho.Contract.Outbox.Builder
{
    internal partial class OutboxBuilder : IOutboxConsumer, IEventOutboxConsumer, IRequestOutboxConsumer, IOutboxDefinition, IEventOutboxDefinition, IRequestOutboxDefinition
    {
        IOutboxDefinition IRequestOutboxDefinition.Declare<Request>()
        {
            AddMessageDefinition(typeof(Request), nameof(Request));
            return this;
        }

        IOutboxDefinition IRequestOutboxDefinition.Declare<Request, Response>()
        {
            AddMessageDefinition(typeof(Request), nameof(Request));
            return this;
        }

        IOutboxConsumer IRequestOutboxConsumer.Handle<Request>(IRequestHandler<Request> handler)
        {
            RegisterRequestHandler(handler);
            return this;
        }

        IOutboxConsumer IRequestOutboxConsumer.Handle<Request, Response>(IRequestHandler<Request, Response> handler)
        {
            RegisterRequestHandler(handler);
            return this;
        }

        IOutboxConsumer IRequestOutboxConsumer.Handle<Request, Handler>()
        {
            Func<Handler> handlerCreator = () => _instanceCreator.CreateInstance<Handler>();
            var handler = new TransientRequestHandler<Request>(handlerCreator);
            RegisterRequestHandler(handler);
            return this;
        }

        IOutboxConsumer IRequestOutboxConsumer.Handle<Request, Response, Handler>()
        {
            Func<Handler> handlerCreator = () => _instanceCreator.CreateInstance<Handler>();
            var handler = new TransientRequestHandler<Request, Response>(handlerCreator);
            RegisterRequestHandler(handler);
            return this;
        }

        IOutboxConsumer IRequestOutboxConsumer.Forward<Request, Module>()
        {
            Func<Request, Request> mapping = requestData => requestData;
            Func<IRequestHandler<Request>> handlerCreator = () =>
            {
                return _instanceCreator.CreateInstance<RequestDownForwarder<Request, Request, Module>>(mapping);
            };
            var handler = new TransientRequestHandler<Request>(handlerCreator);
            RegisterRequestHandler(handler);
            return this;
        }

        IOutboxConsumer IRequestOutboxConsumer.Forward<Request, Response, Module>()
        {
            Func<Request, Request> requestMapping = requestData => requestData;
            Func<Response, Response> responseMapping = response => response;
            Func<IRequestHandler<Request, Response>> handlerCreator = () =>
            {
                return _instanceCreator.CreateInstance<RequestDownForwarder<Request, Response, Request, Response, Module>>(requestMapping, responseMapping);
            };
            var handler = new TransientRequestHandler<Request, Response>(handlerCreator);
            RegisterRequestHandler(handler);
            return this;
        }

        IOutboxConsumer IRequestOutboxConsumer.Forward<RequestIn, RequestOut, Module>(Func<RequestIn, RequestOut> mapping)
        {
            Func<IRequestHandler<RequestIn>> handlerCreator = () =>
            {
                return _instanceCreator.CreateInstance<RequestDownForwarder<RequestIn, RequestOut, Module>>(mapping);
            };
            var handler = new TransientRequestHandler<RequestIn>(handlerCreator);
            RegisterRequestHandler(handler);
            return this;
        }

        IOutboxConsumer IRequestOutboxConsumer.Forward<RequestIn, ResponseIn, RequestOut, ResponseOut, Module>(
            Func<RequestIn, RequestOut> requestMapping,
            Func<ResponseOut, ResponseIn> responseMapping)
        {
            Func<IRequestHandler<RequestIn, ResponseIn>> handlerCreator = () =>
            {
                return _instanceCreator.CreateInstance<RequestDownForwarder<RequestIn, ResponseIn, RequestOut, ResponseOut, Module>>(requestMapping, responseMapping);
            };
            var handler = new TransientRequestHandler<RequestIn, ResponseIn>(handlerCreator);
            RegisterRequestHandler(handler);
            return this;
        }

        IOutboxConsumer IRequestOutboxConsumer.ForwardWithInterception<Request, Interceptor, Module>()
        {
            Func<Request, Request> mapping = requestData => requestData;
            Func<IRequestHandler<Request>> handlerCreator = () =>
            {
                var interceptor = _instanceCreator.CreateInstance<Interceptor>();
                return _instanceCreator.CreateInstance<RequestDownForwarder<Request, Request, Module>>(mapping, interceptor);
            };
            var handler = new TransientRequestHandler<Request>(handlerCreator);
            RegisterRequestHandler(handler);
            return this;
        }

        IOutboxConsumer IRequestOutboxConsumer.ForwardWithInterception<Request, Response, Interceptor, Module>()
        {
            Func<Request, Request> requestMapping = requestData => requestData;
            Func<Response, Response> responseMapping = response => response;
            Func<IRequestHandler<Request, Response>> handlerCreator = () =>
            {
                var interceptor = _instanceCreator.CreateInstance<Interceptor>();
                return _instanceCreator.CreateInstance<RequestDownForwarder<Request, Response, Request, Response, Module>>(requestMapping, responseMapping, interceptor);
            };
            var handler = new TransientRequestHandler<Request, Response>(handlerCreator);
            RegisterRequestHandler(handler);
            return this;
        }

        IOutboxConsumer IRequestOutboxConsumer.ForwardWithInterception<RequestIn, RequestOut, Interceptor, Module>(Func<RequestIn, RequestOut> requestMapping)
        {
            Func<IRequestHandler<RequestIn>> handlerCreator = () =>
            {
                var interceptor = _instanceCreator.CreateInstance<Interceptor>();
                return _instanceCreator.CreateInstance<RequestDownForwarder<RequestIn, RequestOut, Module>>(requestMapping, interceptor);
            };
            var handler = new TransientRequestHandler<RequestIn>(handlerCreator);
            RegisterRequestHandler(handler);
            return this;
        }

        IOutboxConsumer IRequestOutboxConsumer.ForwardWithInterception<RequestIn, ResponseIn, RequestOut, ResponseOut, Interceptor, Module>(
            Func<RequestIn, RequestOut> requestMapping,
            Func<ResponseOut, ResponseIn> responseMapping)
        {
            Func<IRequestHandler<RequestIn, ResponseIn>> handlerCreator = () =>
            {
                var interceptor = _instanceCreator.CreateInstance<Interceptor>();
                return _instanceCreator.CreateInstance<RequestDownForwarder<RequestIn, ResponseIn, RequestOut, ResponseOut, Module>>(requestMapping, responseMapping, interceptor);
            };
            var handler = new TransientRequestHandler<RequestIn, ResponseIn>(handlerCreator);
            RegisterRequestHandler(handler);
            return this;
        }

        IOutboxConsumer IRequestOutboxConsumer.Expose<Request>()
        {
            Func<Request, Request> mapping = requestData => requestData;
            Func<IRequestHandler<Request>> handlerCreator = () =>
            {
                return _instanceCreator.CreateInstance<RequestUpForwarder<Request, Request>>(mapping);
            };
            var handler = new TransientRequestHandler<Request>(handlerCreator);
            RegisterRequestHandler(handler);
            return this;
        }

        IOutboxConsumer IRequestOutboxConsumer.Expose<Request, Response>()
        {
            Func<Request, Request> requestMapping = requestData => requestData;
            Func<Response, Response> responseMapping = response => response;
            Func<IRequestHandler<Request, Response>> handlerCreator = () =>
            {
                return _instanceCreator.CreateInstance<RequestUpForwarder<Request, Response, Request, Response>>(requestMapping, responseMapping);
            };
            var handler = new TransientRequestHandler<Request, Response>(handlerCreator);
            RegisterRequestHandler(handler);
            return this;
        }

        IOutboxConsumer IRequestOutboxConsumer.Expose<RequestIn, RequestOut>(Func<RequestIn, RequestOut> mapping)
        {
            Func<IRequestHandler<RequestIn>> handlerCreator = () =>
            {
                return _instanceCreator.CreateInstance<RequestUpForwarder<RequestIn, RequestOut>>(mapping);
            };
            var handler = new TransientRequestHandler<RequestIn>(handlerCreator);
            RegisterRequestHandler(handler);
            return this;

        }

        IOutboxConsumer IRequestOutboxConsumer.Expose<RequestIn, ResponseIn, RequestOut, ResponseOut>(
            Func<RequestIn, RequestOut> requestMapping,
            Func<ResponseOut, ResponseIn> responseMapping)
        {
            Func<IRequestHandler<RequestIn, ResponseIn>> handlerCreator = () =>
            {
                return _instanceCreator.CreateInstance<RequestUpForwarder<RequestIn, ResponseIn, RequestOut, ResponseOut>>(requestMapping, responseMapping);
            };
            var handler = new TransientRequestHandler<RequestIn, ResponseIn>(handlerCreator);
            RegisterRequestHandler(handler);
            return this;
        }

        IOutboxConsumer IRequestOutboxConsumer.ExposeWithInterception<Request, Interceptor>()
        {
            Func<Request, Request> mapping = requestData => requestData;
            Func<IRequestHandler<Request>> handlerCreator = () =>
            {
                var interceptor = _instanceCreator.CreateInstance<Interceptor>();
                return _instanceCreator.CreateInstance<RequestUpForwarder<Request, Request>>(mapping, interceptor);
            };
            var handler = new TransientRequestHandler<Request>(handlerCreator);
            RegisterRequestHandler(handler);
            return this;
        }

        IOutboxConsumer IRequestOutboxConsumer.ExposeWithInterception<Request, Response, Interceptor>()
        {
            Func<Request, Request> requestMapping = requestData => requestData;
            Func<Response, Response> responseMapping = response => response;
            Func<IRequestHandler<Request, Response>> handlerCreator = () =>
            {
                var interceptor = _instanceCreator.CreateInstance<Interceptor>();
                return _instanceCreator.CreateInstance<RequestUpForwarder<Request, Response, Request, Response>>(requestMapping, responseMapping, interceptor);
            };
            var handler = new TransientRequestHandler<Request, Response>(handlerCreator);
            RegisterRequestHandler(handler);
            return this;
        }

        IOutboxConsumer IRequestOutboxConsumer.ExposeWithInterception<RequestIn, RequestOut, Interceptor>(Func<RequestIn, RequestOut> mapping)
        {
            Func<IRequestHandler<RequestIn>> handlerCreator = () =>
            {
                var interceptor = _instanceCreator.CreateInstance<Interceptor>();
                return _instanceCreator.CreateInstance<RequestUpForwarder<RequestIn, RequestOut>>(mapping, interceptor);
            };
            var handler = new TransientRequestHandler<RequestIn>(handlerCreator);
            RegisterRequestHandler(handler);
            return this;

        }

        IOutboxConsumer IRequestOutboxConsumer.ExposeWithInterception<RequestIn, ResponseIn, RequestOut, ResponseOut, Interceptor>(
            Func<RequestIn, RequestOut> requestMapping,
            Func<ResponseOut, ResponseIn> responseMapping)
        {
            Func<IRequestHandler<RequestIn, ResponseIn>> handlerCreator = () =>
            {
                var interceptor = _instanceCreator.CreateInstance<Interceptor>();
                return _instanceCreator.CreateInstance<RequestUpForwarder<RequestIn, ResponseIn, RequestOut, ResponseOut>>(requestMapping, responseMapping, interceptor);
            };
            var handler = new TransientRequestHandler<RequestIn, ResponseIn>(handlerCreator);
            RegisterRequestHandler(handler);
            return this;
        }

        IOutboxConsumer IRequestOutboxConsumer.Ignore<Request>()
        {
            var handler = new StubRequestHandler<Request>();
            RegisterRequestHandler(handler);
            return this;
        }

        IOutboxConsumer IRequestOutboxConsumer.Ignore<Request, Response>(Response response)
        {
            var handler = new StubRequestHandler<Request, Response>(response);
            RegisterRequestHandler(handler);
            return this;
        }

        private void RegisterRequestHandler<Request>(IRequestHandler<Request> handler)
            where Request : class, IRequest
        {
            ValidateIfMessageIsDefined(typeof(Request), nameof(Request));
            _moduleInbox.RegisterRequestHandler(handler);
            MarkMessageAsHandled(typeof(Request));
        }

        private void RegisterRequestHandler<Request, Response>(IRequestHandler<Request, Response> handler)
            where Request : class, IRequest<Response>
        {
            ValidateIfMessageIsDefined(typeof(Request), nameof(Request));
            _moduleInbox.RegisterRequestWithResponseHandler(handler);
            MarkMessageAsHandled(typeof(Request));
        }
    }
}
