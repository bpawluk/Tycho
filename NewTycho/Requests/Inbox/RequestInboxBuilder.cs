using Microsoft.Extensions.DependencyInjection;
using NewTycho.Requests;
using NewTycho.Requests.Handling;

namespace NewTycho.Requests.Inbox
{
    internal class RequestInboxBuilder : IRequestInboxDefinition
    {
        private readonly IServiceCollection _services;

        public RequestInboxBuilder(IServiceCollection services)
        {
            _services = services;
        }

        IRequestInboxDefinition IRequestInboxDefinition.Forward<TRequest, TModule>()
        {
            _services.AddTransient<IHandle<TRequest>, RequestForwarder<TRequest, TModule>>();
            return this;
        }

        IRequestInboxDefinition IRequestInboxDefinition.Forward<TRequest, TResponse, TModule>()
        {
            _services.AddTransient<IHandle<TRequest, TResponse>, RequestForwarder<TRequest, TResponse, TModule>>();
            return this;
        }

        IRequestInboxDefinition IRequestInboxDefinition.Handle<TRequest, THandler>()
        {
            _services.AddTransient<IHandle<TRequest>, THandler>();
            return this;
        }

        IRequestInboxDefinition IRequestInboxDefinition.Handle<TRequest, TResponse, THandler>()
        {
            _services.AddTransient<IHandle<TRequest, TResponse>, THandler>();
            return this;
        }
    }
}
