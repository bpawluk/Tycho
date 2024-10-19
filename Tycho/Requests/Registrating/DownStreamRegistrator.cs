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
        public void ExposeDownStreamRequest<TSourceModule, TRequest>()
            where TSourceModule : TychoModule
            where TRequest : class, IRequest
        {
            AddDownStreamRegistration<TSourceModule, TRequest, 
                RequestExposer<TRequest>>();
            Services.TryAddTransient<
                RequestExposer<TRequest>>();
        }

        public void ExposeDownStreamRequest<TSourceModule, TRequest, TResponse>()
            where TSourceModule : TychoModule
            where TRequest : class, IRequest<TResponse>
        {
            AddDownStreamRegistration<TSourceModule, TRequest, TResponse, 
                RequestExposer<TRequest, TResponse>>();
            Services.TryAddTransient<
                RequestExposer<TRequest, TResponse>>();
        }

        public void ExposeMappedDownStreamRequest<TSourceModule, TRequest, TTargetRequest>(
            Func<TRequest, TTargetRequest> map)
            where TSourceModule : TychoModule
            where TRequest : class, IRequest
            where TTargetRequest : class, IRequest
        {
            AddDownStreamRegistration<TSourceModule, TRequest,
                MappedRequestExposer<TRequest, TTargetRequest>>();
            Services.TryAddTransient(sp =>
                new MappedRequestExposer<TRequest, TTargetRequest>(
                    sp.GetRequiredService<IParent>(),
                    map));
        }

        public void ExposeMappedDownStreamRequest<TSourceModule, TRequest, TResponse, TTargetRequest, TTargetResponse>(
            Func<TRequest, TTargetRequest> mapRequest,
            Func<TTargetResponse, TResponse> mapResponse)
            where TSourceModule : TychoModule
            where TRequest : class, IRequest<TResponse>
            where TTargetRequest : class, IRequest<TTargetResponse>
        {
            AddDownStreamRegistration<TSourceModule, TRequest, TResponse,
                MappedRequestExposer<TRequest, TResponse, TTargetRequest, TTargetResponse>>();
            Services.TryAddTransient(sp =>
                new MappedRequestExposer<TRequest, TResponse, TTargetRequest, TTargetResponse>(
                    sp.GetRequiredService<IParent>(),
                    mapRequest,
                    mapResponse));
        }

        public void ForwardDownStreamRequest<TSourceModule, TRequest, TTargetModule>()
            where TSourceModule : TychoModule
            where TRequest : class, IRequest
            where TTargetModule : TychoModule
        {
            AddDownStreamRegistration<TSourceModule, TRequest, 
                RequestForwarder<TRequest, TTargetModule>>();
            Services.TryAddTransient<
                RequestForwarder<TRequest, TTargetModule>>();
        }

        public void ForwardDownStreamRequest<TSourceModule, TRequest, TResponse, TTargetModule>()
            where TSourceModule : TychoModule
            where TRequest : class, IRequest<TResponse>
            where TTargetModule : TychoModule
        {
            AddDownStreamRegistration<TSourceModule, TRequest, TResponse,
                RequestForwarder<TRequest, TResponse, TTargetModule>>();
            Services.TryAddTransient<
                RequestForwarder<TRequest, TResponse, TTargetModule>>();
        }

        public void ForwardMappedDownStreamRequest<TSourceModule, TRequest, TTargetRequest, TTargetModule>(
            Func<TRequest, TTargetRequest> map)
            where TSourceModule : TychoModule
            where TRequest : class, IRequest
            where TTargetRequest : class, IRequest
            where TTargetModule : TychoModule
        {
            AddDownStreamRegistration<TSourceModule, TRequest,
                MappedRequestForwarder<TRequest, TTargetRequest, TTargetModule>>();
            Services.TryAddTransient(sp =>
                new MappedRequestForwarder<TRequest, TTargetRequest, TTargetModule>(
                    sp.GetRequiredService<IModule<TTargetModule>>(),
                    map));
        }

        public void ForwardMappedDownStreamRequest<TSourceModule, TRequest, TResponse, TTargetRequest, TTargetResponse, TTargetModule>(
            Func<TRequest, TTargetRequest> mapRequest,
            Func<TTargetResponse, TResponse> mapResponse)
            where TSourceModule : TychoModule
            where TRequest : class, IRequest<TResponse>
            where TTargetRequest : class, IRequest<TTargetResponse>
            where TTargetModule : TychoModule
        {
            AddDownStreamRegistration<TSourceModule, TRequest, TResponse,
                MappedRequestForwarder<TRequest, TResponse, TTargetRequest, TTargetResponse, TTargetModule>>();
            Services.TryAddTransient(sp =>
                new MappedRequestForwarder<TRequest, TResponse, TTargetRequest, TTargetResponse, TTargetModule>(
                    sp.GetRequiredService<IModule<TTargetModule>>(),
                    mapRequest,
                    mapResponse));
        }

        public void IgnoreDownStreamRequest<TSourceModule, TRequest>()
            where TSourceModule : TychoModule
            where TRequest : class, IRequest
        {
            AddDownStreamRegistration<TSourceModule, TRequest, 
                RequestIgnorer<TRequest>>();
            Services.TryAddTransient<
                RequestIgnorer<TRequest>>();
        }

        public void IgnoreDownStreamRequest<TSourceModule, TRequest, TResponse>()
            where TSourceModule : TychoModule
            where TRequest : class, IRequest<TResponse>
        {
            AddDownStreamRegistration<TSourceModule, TRequest, TResponse, 
                RequestIgnorer<TRequest, TResponse>>();
            Services.TryAddTransient<
                RequestIgnorer<TRequest, TResponse>>();
        }

        public void HandleDownStreamRequest<TSourceModule, TRequest, THandler>()
            where TSourceModule : TychoModule
            where TRequest : class, IRequest
            where THandler : class, IRequestHandler<TRequest>
        {
            AddDownStreamRegistration<TSourceModule, TRequest, THandler>();
            Services.TryAddTransient<THandler>();
        }

        public void HandleDownStreamRequest<TSourceModule, TRequest, TResponse, THandler>()
            where TSourceModule : TychoModule
            where TRequest : class, IRequest<TResponse>
            where THandler : class, IRequestHandler<TRequest, TResponse>
        {
            AddDownStreamRegistration<TSourceModule, TRequest, TResponse, THandler>();
            Services.TryAddTransient<THandler>();
        }

        private void AddDownStreamRegistration<TSourceModule, TRequest, THandler>()
            where TSourceModule : TychoModule
            where TRequest : class, IRequest
            where THandler : class, IRequestHandler<TRequest>
        {
            if (!TryAddRegistration<
                    IDownStreamHandlerRegistration<TRequest, TSourceModule>,
                    DownStreamHandlerRegistration<TRequest, THandler, TSourceModule>>())
            {
                throw new ArgumentException(
                    $"Request handler for {typeof(TRequest).Name} already registered",
                    nameof(THandler));
            }
        }

        private void AddDownStreamRegistration<TSourceModule, TRequest, TResponse, THandler>()
            where TSourceModule : TychoModule
            where TRequest : class, IRequest<TResponse>
            where THandler : class, IRequestHandler<TRequest, TResponse>
        {
            if (!TryAddRegistration<
                    IDownStreamHandlerRegistration<TRequest, TResponse, TSourceModule>,
                    DownStreamHandlerRegistration<TRequest, TResponse, THandler, TSourceModule>>())
            {
                throw new ArgumentException(
                    $"Request handler for {typeof(TRequest).Name} already registered",
                    nameof(THandler));
            }
        }
    }
}