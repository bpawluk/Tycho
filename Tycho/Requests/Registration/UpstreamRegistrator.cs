using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Tycho.Modules;
using Tycho.Modules.Instance;
using Tycho.Requests.Handling;
using Tycho.Requests.Registration.Registrations;

namespace Tycho.Requests.Registration
{
    internal partial class Registrator
    {
        public void ForwardUpstreamRequest<TRequest, TTargetModule>()
            where TRequest : class, IRequest
            where TTargetModule : TychoModule
        {
            AddUpstreamRegistration<TRequest, RequestForwarder<TRequest, TTargetModule>>();
            Services.TryAddTransient<RequestForwarder<TRequest, TTargetModule>>();
        }

        public void ForwardUpstreamRequest<TRequest, TResponse, TTargetModule>()
            where TRequest : class, IRequest<TResponse>
            where TTargetModule : TychoModule
        {
            AddUpstreamRegistration<TRequest, TResponse, RequestForwarder<TRequest, TResponse, TTargetModule>>();
            Services.TryAddTransient<RequestForwarder<TRequest, TResponse, TTargetModule>>();
        }

        public void ForwardMappedUpstreamRequest<TRequest, TTargetRequest, TTargetModule>(
            Func<TRequest, TTargetRequest> map)
            where TRequest : class, IRequest
            where TTargetRequest : class, IRequest
            where TTargetModule : TychoModule
        {
            AddUpstreamRegistration<TRequest, MappedRequestForwarder<TRequest, TTargetRequest, TTargetModule>>(sp =>
                new MappedRequestForwarder<TRequest, TTargetRequest, TTargetModule>(
                    sp.GetRequiredService<IModule<TTargetModule>>(),
                    map));
        }

        public void ForwardMappedUpstreamRequest<TRequest, TResponse, TTargetRequest, TTargetResponse, TTargetModule>(
            Func<TRequest, TTargetRequest> mapRequest,
            Func<TTargetResponse, TResponse> mapResponse)
            where TRequest : class, IRequest<TResponse>
            where TTargetRequest : class, IRequest<TTargetResponse>
            where TTargetModule : TychoModule
        {
            AddUpstreamRegistration<
                TRequest, TResponse,
                MappedRequestForwarder<TRequest, TResponse, TTargetRequest, TTargetResponse, TTargetModule>>(sp =>
                new MappedRequestForwarder<TRequest, TResponse, TTargetRequest, TTargetResponse, TTargetModule>(
                    sp.GetRequiredService<IModule<TTargetModule>>(),
                    mapRequest,
                    mapResponse));
        }

        public void HandleUpstreamRequest<TRequest, THandler>()
            where TRequest : class, IRequest
            where THandler : class, IRequestHandler<TRequest>
        {
            AddUpstreamRegistration<TRequest, THandler>();
            Services.TryAddScoped<THandler>();
        }

        public void HandleUpstreamRequest<TRequest, TResponse, THandler>()
            where TRequest : class, IRequest<TResponse>
            where THandler : class, IRequestHandler<TRequest, TResponse>
        {
            AddUpstreamRegistration<TRequest, TResponse, THandler>();
            Services.TryAddScoped<THandler>();
        }

        private void AddUpstreamRegistration<TRequest, THandler>()
            where TRequest : class, IRequest
            where THandler : class, IRequestHandler<TRequest>
        {
            if (!TryAddRegistration<
                    IUpstreamRequestRegistration<TRequest>,
                    UpstreamRequestRegistration<TRequest, THandler>>())
            {
                throw new ArgumentException(
                    $"Request handler for {typeof(TRequest).Name} already registered",
                    nameof(THandler));
            }
        }

        private void AddUpstreamRegistration<TRequest, THandler>(
            Func<IServiceProvider, THandler> handlerFactory)
            where TRequest : class, IRequest
            where THandler : class, IRequestHandler<TRequest>
        {
            if (!TryAddRegistration<
                    IUpstreamRequestRegistration<TRequest>,
                    UpstreamRequestRegistration<TRequest, THandler>>(sp =>
                        new UpstreamRequestRegistration<TRequest, THandler>(handlerFactory(sp))))
            {
                throw new ArgumentException(
                    $"Request handler for {typeof(TRequest).Name} already registered",
                    nameof(THandler));
            }
        }

        private void AddUpstreamRegistration<TRequest, TResponse, THandler>()
            where TRequest : class, IRequest<TResponse>
            where THandler : class, IRequestHandler<TRequest, TResponse>
        {
            if (!TryAddRegistration<
                    IUpstreamRequestRegistration<TRequest, TResponse>,
                    UpstreamRequestRegistration<TRequest, TResponse, THandler>>())
            {
                throw new ArgumentException(
                    $"Request handler for {typeof(TRequest).Name} already registered",
                    nameof(THandler));
            }
        }

        private void AddUpstreamRegistration<TRequest, TResponse, THandler>(
            Func<IServiceProvider, THandler> handlerFactory)
            where TRequest : class, IRequest<TResponse>
            where THandler : class, IRequestHandler<TRequest, TResponse>
        {
            if (!TryAddRegistration<
                    IUpstreamRequestRegistration<TRequest, TResponse>,
                    UpstreamRequestRegistration<TRequest, TResponse, THandler>>(sp =>
                        new UpstreamRequestRegistration<TRequest, TResponse, THandler>(handlerFactory(sp))))
            {
                throw new ArgumentException(
                    $"Request handler for {typeof(TRequest).Name} already registered",
                    nameof(THandler));
            }
        }
    }
}
