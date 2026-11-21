using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Tycho.Modules;
using Tycho.Modules.Instance;
using Tycho.Requests.Handling;
using Tycho.Requests.Registration.Registrations;
using Tycho.Structure.Parent;

namespace Tycho.Requests.Registration
{
    internal partial class Registrator
    {
        public void ExposeDownstreamRequest<TSourceModule, TRequest>()
            where TSourceModule : TychoModule
            where TRequest : class, IRequest
        {
            AddDownstreamRegistration<TSourceModule, TRequest, RequestExposer<TRequest>>();
            Services.TryAddTransient<RequestExposer<TRequest>>();
        }

        public void ExposeDownstreamRequest<TSourceModule, TRequest, TResponse>()
            where TSourceModule : TychoModule
            where TRequest : class, IRequest<TResponse>
        {
            AddDownstreamRegistration<TSourceModule, TRequest, TResponse, RequestExposer<TRequest, TResponse>>();
            Services.TryAddTransient<RequestExposer<TRequest, TResponse>>();
        }

        public void ExposeMappedDownstreamRequest<TSourceModule, TRequest, TTargetRequest>(
            Func<TRequest, TTargetRequest> map)
            where TSourceModule : TychoModule
            where TRequest : class, IRequest
            where TTargetRequest : class, IRequest
        {
            AddDownstreamRegistration<TSourceModule, TRequest, MappedRequestExposer<TRequest, TTargetRequest>>(sp =>
                new MappedRequestExposer<TRequest, TTargetRequest>(
                    sp.GetRequiredService<IParentReference>(),
                    map));
        }

        public void ExposeMappedDownstreamRequest<TSourceModule, TRequest, TResponse, TTargetRequest, TTargetResponse>(
            Func<TRequest, TTargetRequest> mapRequest,
            Func<TTargetResponse, TResponse> mapResponse)
            where TSourceModule : TychoModule
            where TRequest : class, IRequest<TResponse>
            where TTargetRequest : class, IRequest<TTargetResponse>
        {
            AddDownstreamRegistration<
                TSourceModule, TRequest, TResponse,
                MappedRequestExposer<TRequest, TResponse, TTargetRequest, TTargetResponse>>(sp =>
                new MappedRequestExposer<TRequest, TResponse, TTargetRequest, TTargetResponse>(
                    sp.GetRequiredService<IParentReference>(),
                    mapRequest,
                    mapResponse));
        }

        public void ForwardDownstreamRequest<TSourceModule, TRequest, TTargetModule>()
            where TSourceModule : TychoModule
            where TRequest : class, IRequest
            where TTargetModule : TychoModule
        {
            AddDownstreamRegistration<TSourceModule, TRequest, RequestForwarder<TRequest, TTargetModule>>();
            Services.TryAddTransient<RequestForwarder<TRequest, TTargetModule>>();
        }

        public void ForwardDownstreamRequest<TSourceModule, TRequest, TResponse, TTargetModule>()
            where TSourceModule : TychoModule
            where TRequest : class, IRequest<TResponse>
            where TTargetModule : TychoModule
        {
            AddDownstreamRegistration<
                TSourceModule, TRequest, TResponse,
                RequestForwarder<TRequest, TResponse, TTargetModule>>();
            Services.TryAddTransient<RequestForwarder<TRequest, TResponse, TTargetModule>>();
        }

        public void ForwardMappedDownstreamRequest<TSourceModule, TRequest, TTargetRequest, TTargetModule>(
            Func<TRequest, TTargetRequest> map)
            where TSourceModule : TychoModule
            where TRequest : class, IRequest
            where TTargetRequest : class, IRequest
            where TTargetModule : TychoModule
        {
            AddDownstreamRegistration<
                TSourceModule, TRequest,
                MappedRequestForwarder<TRequest, TTargetRequest, TTargetModule>>(sp =>
                new MappedRequestForwarder<TRequest, TTargetRequest, TTargetModule>(
                    sp.GetRequiredService<IModule<TTargetModule>>(),
                    map));
        }

        public void ForwardMappedDownstreamRequest<TSourceModule, TRequest, TResponse, TTargetRequest, TTargetResponse, TTargetModule>(
            Func<TRequest, TTargetRequest> mapRequest,
            Func<TTargetResponse, TResponse> mapResponse)
            where TSourceModule : TychoModule
            where TRequest : class, IRequest<TResponse>
            where TTargetRequest : class, IRequest<TTargetResponse>
            where TTargetModule : TychoModule
        {
            AddDownstreamRegistration<
                TSourceModule, TRequest, TResponse,
                MappedRequestForwarder<TRequest, TResponse, TTargetRequest, TTargetResponse, TTargetModule>>(sp =>
                new MappedRequestForwarder<TRequest, TResponse, TTargetRequest, TTargetResponse, TTargetModule>(
                    sp.GetRequiredService<IModule<TTargetModule>>(),
                    mapRequest,
                    mapResponse));
        }

        public void IgnoreDownstreamRequest<TSourceModule, TRequest>()
            where TSourceModule : TychoModule
            where TRequest : class, IRequest
        {
            AddDownstreamRegistration<TSourceModule, TRequest, RequestIgnorer<TRequest>>();
            Services.TryAddTransient<RequestIgnorer<TRequest>>();
        }

        public void IgnoreDownstreamRequest<TSourceModule, TRequest, TResponse>()
            where TSourceModule : TychoModule
            where TRequest : class, IRequest<TResponse>
        {
            AddDownstreamRegistration<TSourceModule, TRequest, TResponse, RequestIgnorer<TRequest, TResponse>>();
            Services.TryAddTransient<RequestIgnorer<TRequest, TResponse>>();
        }

        public void HandleDownstreamRequest<TSourceModule, TRequest, THandler>()
            where TSourceModule : TychoModule
            where TRequest : class, IRequest
            where THandler : class, IRequestHandler<TRequest>
        {
            AddDownstreamRegistration<TSourceModule, TRequest, THandler>();
            Services.TryAddScoped<THandler>();
        }

        public void HandleDownstreamRequest<TSourceModule, TRequest, TResponse, THandler>()
            where TSourceModule : TychoModule
            where TRequest : class, IRequest<TResponse>
            where THandler : class, IRequestHandler<TRequest, TResponse>
        {
            AddDownstreamRegistration<TSourceModule, TRequest, TResponse, THandler>();
            Services.TryAddScoped<THandler>();
        }

        private void AddDownstreamRegistration<TSourceModule, TRequest, THandler>()
            where TSourceModule : TychoModule
            where TRequest : class, IRequest
            where THandler : class, IRequestHandler<TRequest>
        {
            if (!TryAddRegistration<
                    IDownstreamRequestRegistration<TRequest, TSourceModule>,
                    DownstreamRequestRegistration<TRequest, THandler, TSourceModule>>())
            {
                throw new ArgumentException(
                    $"Request handler for {typeof(TRequest).Name} is already registered",
                    nameof(THandler));
            }
        }

        private void AddDownstreamRegistration<TSourceModule, TRequest, THandler>(
            Func<IServiceProvider, THandler> handlerFactory)
            where TSourceModule : TychoModule
            where TRequest : class, IRequest
            where THandler : class, IRequestHandler<TRequest>
        {
            if (!TryAddRegistration<
                    IDownstreamRequestRegistration<TRequest, TSourceModule>,
                    DownstreamRequestRegistration<TRequest, THandler, TSourceModule>>(sp =>
                        new DownstreamRequestRegistration<TRequest, THandler, TSourceModule>(handlerFactory(sp))))
            {
                throw new ArgumentException(
                    $"Request handler for {typeof(TRequest).Name} is already registered",
                    nameof(THandler));
            }
        }

        private void AddDownstreamRegistration<TSourceModule, TRequest, TResponse, THandler>()
            where TSourceModule : TychoModule
            where TRequest : class, IRequest<TResponse>
            where THandler : class, IRequestHandler<TRequest, TResponse>
        {
            if (!TryAddRegistration<
                    IDownstreamRequestRegistration<TRequest, TResponse, TSourceModule>,
                    DownstreamRequestRegistration<TRequest, TResponse, THandler, TSourceModule>>())
            {
                throw new ArgumentException(
                    $"Request handler for {typeof(TRequest).Name} is already registered",
                    nameof(THandler));
            }
        }

        private void AddDownstreamRegistration<TSourceModule, TRequest, TResponse, THandler>(
            Func<IServiceProvider, THandler> handlerFactory)
            where TSourceModule : TychoModule
            where TRequest : class, IRequest<TResponse>
            where THandler : class, IRequestHandler<TRequest, TResponse>
        {
            if (!TryAddRegistration<
                    IDownstreamRequestRegistration<TRequest, TResponse, TSourceModule>,
                    DownstreamRequestRegistration<TRequest, TResponse, THandler, TSourceModule>>(sp =>
                        new DownstreamRequestRegistration<TRequest, TResponse, THandler, TSourceModule>(handlerFactory(sp))))
            {
                throw new ArgumentException(
                    $"Request handler for {typeof(TRequest).Name} is already registered",
                    nameof(THandler));
            }
        }
    }
}
