using System;
using System.Threading;
using System.Threading.Tasks;
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

        IOutboxConsumer IRequestOutboxConsumer.Handle<Request>(Action<Request> action)
        {
            var handler = new LambdaWrappingCommandHandler<Request>(action);
            RegisterRequestHandler(handler);
            return this;
        }

        IOutboxConsumer IRequestOutboxConsumer.Handle<Request, Response>(Func<Request, Response> function)
        {
            var handler = new LambdaWrappingQueryHandler<Request, Response>(function);
            RegisterRequestHandler(handler);
            return this;
        }

        IOutboxConsumer IRequestOutboxConsumer.Handle<Request>(Func<Request, Task> function)
        {
            var handler = new LambdaWrappingCommandHandler<Request>(function);
            RegisterRequestHandler(handler);
            return this;
        }

        IOutboxConsumer IRequestOutboxConsumer.Handle<Request, Response>(Func<Request, Task<Response>> function)
        {
            var handler = new LambdaWrappingQueryHandler<Request, Response>(function);
            RegisterRequestHandler(handler);
            return this;
        }

        IOutboxConsumer IRequestOutboxConsumer.Handle<Request>(Func<Request, CancellationToken, Task> function)
        {
            var handler = new LambdaWrappingCommandHandler<Request>(function);
            RegisterRequestHandler(handler);
            return this;
        }

        IOutboxConsumer IRequestOutboxConsumer.Handle<Request, Response>(Func<Request, CancellationToken, Task<Response>> function)
        {
            var handler = new LambdaWrappingQueryHandler<Request, Response>(function);
            RegisterRequestHandler(handler);
            return this;
        }

        IOutboxConsumer IRequestOutboxConsumer.Handle<Request>(ICommandHandler<Request> handler)
        {
            RegisterRequestHandler(handler);
            return this;
        }

        IOutboxConsumer IRequestOutboxConsumer.Handle<Request, Response>(IQueryHandler<Request, Response> handler)
        {
            RegisterRequestHandler(handler);
            return this;
        }

        IOutboxConsumer IRequestOutboxConsumer.Handle<Request, Handler>()
        {
            Func<Handler> handlerCreator = () => _instanceCreator.CreateInstance<Handler>();
            var handler = new TransientCommandHandler<Request>(handlerCreator);
            RegisterRequestHandler(handler);
            return this;
        }

        IOutboxConsumer IRequestOutboxConsumer.Handle<Request, Response, Handler>()
        {
            Func<Handler> handlerCreator = () => _instanceCreator.CreateInstance<Handler>();
            var handler = new TransientQueryHandler<Request, Response>(handlerCreator);
            RegisterRequestHandler(handler);
            return this;
        }

        IOutboxConsumer IRequestOutboxConsumer.Forward<Request, Module>()
        {
            Func<Request, Request> mapping = requestData => requestData;
            Func<ICommandHandler<Request>> handlerCreator = () =>
            {
                return _instanceCreator.CreateInstance<CommandDownForwarder<Request, Request, Module>>(mapping);
            };
            var handler = new TransientCommandHandler<Request>(handlerCreator);
            RegisterRequestHandler(handler);
            return this;
        }

        IOutboxConsumer IRequestOutboxConsumer.Forward<Request, Response, Module>()
        {
            Func<Request, Request> requestMapping = requestData => requestData;
            Func<Response, Response> responseMapping = response => response;
            Func<IQueryHandler<Request, Response>> handlerCreator = () =>
            {
                return _instanceCreator.CreateInstance<QueryDownForwarder<Request, Response, Request, Response, Module>>(requestMapping, responseMapping);
            };
            var handler = new TransientQueryHandler<Request, Response>(handlerCreator);
            RegisterRequestHandler(handler);
            return this;
        }

        IOutboxConsumer IRequestOutboxConsumer.Forward<RequestIn, RequestOut, Module>(Func<RequestIn, RequestOut> mapping)
        {
            Func<ICommandHandler<RequestIn>> handlerCreator = () =>
            {
                return _instanceCreator.CreateInstance<CommandDownForwarder<RequestIn, RequestOut, Module>>(mapping);
            };
            var handler = new TransientCommandHandler<RequestIn>(handlerCreator);
            RegisterRequestHandler(handler);
            return this;
        }

        IOutboxConsumer IRequestOutboxConsumer.Forward<RequestIn, ResponseIn, RequestOut, ResponseOut, Module>(
            Func<RequestIn, RequestOut> requestMapping,
            Func<ResponseOut, ResponseIn> responseMapping)
        {
            Func<IQueryHandler<RequestIn, ResponseIn>> handlerCreator = () =>
            {
                return _instanceCreator.CreateInstance<QueryDownForwarder<RequestIn, ResponseIn, RequestOut, ResponseOut, Module>>(requestMapping, responseMapping);
            };
            var handler = new TransientQueryHandler<RequestIn, ResponseIn>(handlerCreator);
            RegisterRequestHandler(handler);
            return this;
        }

        IOutboxConsumer IRequestOutboxConsumer.ForwardWithInterception<Request, Interceptor, Module>()
        {
            Func<Request, Request> mapping = requestData => requestData;
            Func<ICommandHandler<Request>> handlerCreator = () =>
            {
                var interceptor = _instanceCreator.CreateInstance<Interceptor>();
                return _instanceCreator.CreateInstance<CommandDownForwarder<Request, Request, Module>>(mapping, interceptor);
            };
            var handler = new TransientCommandHandler<Request>(handlerCreator);
            RegisterRequestHandler(handler);
            return this;
        }

        IOutboxConsumer IRequestOutboxConsumer.ForwardWithInterception<Request, Response, Interceptor, Module>()
        {
            Func<Request, Request> requestMapping = requestData => requestData;
            Func<Response, Response> responseMapping = response => response;
            Func<IQueryHandler<Request, Response>> handlerCreator = () =>
            {
                var interceptor = _instanceCreator.CreateInstance<Interceptor>();
                return _instanceCreator.CreateInstance<QueryDownForwarder<Request, Response, Request, Response, Module>>(requestMapping, responseMapping, interceptor);
            };
            var handler = new TransientQueryHandler<Request, Response>(handlerCreator);
            RegisterRequestHandler(handler);
            return this;
        }

        IOutboxConsumer IRequestOutboxConsumer.ForwardWithInterception<RequestIn, RequestOut, Interceptor, Module>(Func<RequestIn, RequestOut> requestMapping)
        {
            Func<ICommandHandler<RequestIn>> handlerCreator = () =>
            {
                var interceptor = _instanceCreator.CreateInstance<Interceptor>();
                return _instanceCreator.CreateInstance<CommandDownForwarder<RequestIn, RequestOut, Module>>(requestMapping, interceptor);
            };
            var handler = new TransientCommandHandler<RequestIn>(handlerCreator);
            RegisterRequestHandler(handler);
            return this;
        }

        IOutboxConsumer IRequestOutboxConsumer.ForwardWithInterception<RequestIn, ResponseIn, RequestOut, ResponseOut, Interceptor, Module>(
            Func<RequestIn, RequestOut> requestMapping,
            Func<ResponseOut, ResponseIn> responseMapping)
        {
            Func<IQueryHandler<RequestIn, ResponseIn>> handlerCreator = () =>
            {
                var interceptor = _instanceCreator.CreateInstance<Interceptor>();
                return _instanceCreator.CreateInstance<QueryDownForwarder<RequestIn, ResponseIn, RequestOut, ResponseOut, Module>>(requestMapping, responseMapping, interceptor);
            };
            var handler = new TransientQueryHandler<RequestIn, ResponseIn>(handlerCreator);
            RegisterRequestHandler(handler);
            return this;
        }

        IOutboxConsumer IRequestOutboxConsumer.Expose<Request>()
        {
            Func<Request, Request> mapping = requestData => requestData;
            Func<ICommandHandler<Request>> handlerCreator = () =>
            {
                return _instanceCreator.CreateInstance<CommandUpForwarder<Request, Request>>(mapping);
            };
            var handler = new TransientCommandHandler<Request>(handlerCreator);
            RegisterRequestHandler(handler);
            return this;
        }

        IOutboxConsumer IRequestOutboxConsumer.Expose<Request, Response>()
        {
            Func<Request, Request> requestMapping = requestData => requestData;
            Func<Response, Response> responseMapping = response => response;
            Func<IQueryHandler<Request, Response>> handlerCreator = () =>
            {
                return _instanceCreator.CreateInstance<QueryUpForwarder<Request, Response, Request, Response>>(requestMapping, responseMapping);
            };
            var handler = new TransientQueryHandler<Request, Response>(handlerCreator);
            RegisterRequestHandler(handler);
            return this;
        }

        IOutboxConsumer IRequestOutboxConsumer.Expose<RequestIn, RequestOut>(Func<RequestIn, RequestOut> mapping)
        {
            Func<ICommandHandler<RequestIn>> handlerCreator = () =>
            {
                return _instanceCreator.CreateInstance<CommandUpForwarder<RequestIn, RequestOut>>(mapping);
            };
            var handler = new TransientCommandHandler<RequestIn>(handlerCreator);
            RegisterRequestHandler(handler);
            return this;

        }

        IOutboxConsumer IRequestOutboxConsumer.Expose<RequestIn, ResponseIn, RequestOut, ResponseOut>(
            Func<RequestIn, RequestOut> requestMapping,
            Func<ResponseOut, ResponseIn> responseMapping)
        {
            Func<IQueryHandler<RequestIn, ResponseIn>> handlerCreator = () =>
            {
                return _instanceCreator.CreateInstance<QueryUpForwarder<RequestIn, ResponseIn, RequestOut, ResponseOut>>(requestMapping, responseMapping);
            };
            var handler = new TransientQueryHandler<RequestIn, ResponseIn>(handlerCreator);
            RegisterRequestHandler(handler);
            return this;
        }

        IOutboxConsumer IRequestOutboxConsumer.ExposeWithInterception<Request, Interceptor>()
        {
            Func<Request, Request> mapping = requestData => requestData;
            Func<ICommandHandler<Request>> handlerCreator = () =>
            {
                var interceptor = _instanceCreator.CreateInstance<Interceptor>();
                return _instanceCreator.CreateInstance<CommandUpForwarder<Request, Request>>(mapping, interceptor);
            };
            var handler = new TransientCommandHandler<Request>(handlerCreator);
            RegisterRequestHandler(handler);
            return this;
        }

        IOutboxConsumer IRequestOutboxConsumer.ExposeWithInterception<Request, Response, Interceptor>()
        {
            Func<Request, Request> requestMapping = requestData => requestData;
            Func<Response, Response> responseMapping = response => response;
            Func<IQueryHandler<Request, Response>> handlerCreator = () =>
            {
                var interceptor = _instanceCreator.CreateInstance<Interceptor>();
                return _instanceCreator.CreateInstance<QueryUpForwarder<Request, Response, Request, Response>>(requestMapping, responseMapping, interceptor);
            };
            var handler = new TransientQueryHandler<Request, Response>(handlerCreator);
            RegisterRequestHandler(handler);
            return this;
        }

        IOutboxConsumer IRequestOutboxConsumer.ExposeWithInterception<RequestIn, RequestOut, Interceptor>(Func<RequestIn, RequestOut> mapping)
        {
            Func<ICommandHandler<RequestIn>> handlerCreator = () =>
            {
                var interceptor = _instanceCreator.CreateInstance<Interceptor>();
                return _instanceCreator.CreateInstance<CommandUpForwarder<RequestIn, RequestOut>>(mapping, interceptor);
            };
            var handler = new TransientCommandHandler<RequestIn>(handlerCreator);
            RegisterRequestHandler(handler);
            return this;

        }

        IOutboxConsumer IRequestOutboxConsumer.ExposeWithInterception<RequestIn, ResponseIn, RequestOut, ResponseOut, Interceptor>(
            Func<RequestIn, RequestOut> requestMapping,
            Func<ResponseOut, ResponseIn> responseMapping)
        {
            Func<IQueryHandler<RequestIn, ResponseIn>> handlerCreator = () =>
            {
                var interceptor = _instanceCreator.CreateInstance<Interceptor>();
                return _instanceCreator.CreateInstance<QueryUpForwarder<RequestIn, ResponseIn, RequestOut, ResponseOut>>(requestMapping, responseMapping, interceptor);
            };
            var handler = new TransientQueryHandler<RequestIn, ResponseIn>(handlerCreator);
            RegisterRequestHandler(handler);
            return this;
        }

        IOutboxConsumer IRequestOutboxConsumer.Ignore<Request>()
        {
            var handler = new StubCommandHandler<Request>();
            RegisterRequestHandler(handler);
            return this;
        }

        IOutboxConsumer IRequestOutboxConsumer.Ignore<Request, Response>(Response response)
        {
            var handler = new StubQueryHandler<Request, Response>(response);
            RegisterRequestHandler(handler);
            return this;
        }

        private void RegisterRequestHandler<Request>(ICommandHandler<Request> handler)
            where Request : class, IRequest
        {
            ValidateIfMessageIsDefined(typeof(Request), nameof(Request));
            _moduleInbox.RegisterCommandHandler(handler);
            MarkMessageAsHandled(typeof(Request));
        }

        private void RegisterRequestHandler<Request, Response>(IQueryHandler<Request, Response> handler)
            where Request : class, IRequest<Response>
        {
            ValidateIfMessageIsDefined(typeof(Request), nameof(Request));
            _moduleInbox.RegisterQueryHandler(handler);
            MarkMessageAsHandled(typeof(Request));
        }
    }
}
