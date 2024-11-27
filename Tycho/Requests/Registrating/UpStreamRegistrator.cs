using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Tycho.Modules;
using Tycho.Requests.Handling;
using Tycho.Requests.Registrating.Registrations;
using Tycho.Structure;

namespace Tycho.Requests.Registrating
{
    internal partial class Registrator
    {
        public void ForwardUpStreamRequest<TRequest, TTargetModule>()
            where TRequest : class, IRequest
            where TTargetModule : TychoModule
        {
            AddUpStreamRegistration<TRequest, RequestForwarder<TRequest, TTargetModule>>();
            Services.TryAddTransient<RequestForwarder<TRequest, TTargetModule>>();
        }

        public void ForwardUpStreamRequest<TRequest, TResponse, TTargetModule>()
            where TRequest : class, IRequest<TResponse>
            where TTargetModule : TychoModule
        {
            AddUpStreamRegistration<TRequest, TResponse, RequestForwarder<TRequest, TResponse, TTargetModule>>();
            Services.TryAddTransient<RequestForwarder<TRequest, TResponse, TTargetModule>>();
        }

        public void ForwardMappedUpStreamRequest<TRequest, TTargetRequest, TTargetModule>(
            Func<TRequest, TTargetRequest> map)
            where TRequest : class, IRequest
            where TTargetRequest : class, IRequest
            where TTargetModule : TychoModule
        {
            AddUpStreamRegistration<TRequest, MappedRequestForwarder<TRequest, TTargetRequest, TTargetModule>>();
            Services.TryAddTransient(sp =>
                new MappedRequestForwarder<TRequest, TTargetRequest, TTargetModule>(
                    sp.GetRequiredService<IModule<TTargetModule>>(), 
                    map));
        }

        public void ForwardMappedUpStreamRequest<TRequest, TResponse, TTargetRequest, TTargetResponse, TTargetModule>(
            Func<TRequest, TTargetRequest> mapRequest,
            Func<TTargetResponse, TResponse> mapResponse)
            where TRequest : class, IRequest<TResponse>
            where TTargetRequest : class, IRequest<TTargetResponse>
            where TTargetModule : TychoModule
        {
            AddUpStreamRegistration<
                TRequest, TResponse, 
                MappedRequestForwarder<TRequest, TResponse, TTargetRequest, TTargetResponse, TTargetModule>>();
            Services.TryAddTransient(sp =>
                new MappedRequestForwarder<TRequest, TResponse, TTargetRequest, TTargetResponse, TTargetModule>(
                    sp.GetRequiredService<IModule<TTargetModule>>(),
                    mapRequest,
                    mapResponse));
        }

        public void HandleUpStreamRequest<TRequest, THandler>()
            where TRequest : class, IRequest
            where THandler : class, IRequestHandler<TRequest>
        {
            AddUpStreamRegistration<TRequest, ScopedRequestHandler<TRequest, THandler>>();
            Services.TryAddTransient<ScopedRequestHandler<TRequest, THandler>>();
            Services.TryAddScoped<THandler>();
        }

        public void HandleUpStreamRequest<TRequest, TResponse, THandler>()
            where TRequest : class, IRequest<TResponse>
            where THandler : class, IRequestHandler<TRequest, TResponse>
        {
            AddUpStreamRegistration<TRequest, TResponse, ScopedRequestHandler<TRequest, TResponse, THandler>>();
            Services.TryAddTransient<ScopedRequestHandler<TRequest, TResponse, THandler>>();
            Services.TryAddScoped<THandler>();
        }

        private void AddUpStreamRegistration<TRequest, THandler>()
            where TRequest : class, IRequest
            where THandler : class, IRequestHandler<TRequest>
        {
            if (!TryAddRegistration<
                    IUpStreamHandlerRegistration<TRequest>,
                    UpStreamHandlerRegistration<TRequest, THandler>>())
            {
                throw new ArgumentException(
                    $"Request handler for {typeof(TRequest).Name} already registered",
                    nameof(THandler));
            }
        }

        private void AddUpStreamRegistration<TRequest, TResponse, THandler>()
            where TRequest : class, IRequest<TResponse>
            where THandler : class, IRequestHandler<TRequest, TResponse>
        {
            if (!TryAddRegistration<
                    IUpStreamHandlerRegistration<TRequest, TResponse>,
                    UpStreamHandlerRegistration<TRequest, TResponse, THandler>>())
            {
                throw new ArgumentException(
                    $"Request handler for {typeof(TRequest).Name} already registered",
                    nameof(THandler));
            }
        }
    }
}