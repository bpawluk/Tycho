using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using TychoV2.Requests;
using TychoV2.Structure;

namespace TychoV2.Modules.Instance
{
    internal class Module<TModuleDefinition> : IModule<TModuleDefinition>
        where TModuleDefinition : TychoModule
    {
        private readonly Internals _internals;

        Internals IModule.Internals => _internals;

        public Module(Internals internals)
        {
            _internals = internals;
        }

        public Task Execute<TRequest>(TRequest requestData, CancellationToken cancellationToken)
             where TRequest : class, IRequest
        {
            var handler = _internals.GetService<IHandle<TRequest>>() ?? throw new InvalidOperationException($"Request handler for {typeof(TRequest).Name} was not registered");
            return handler.Handle(requestData, cancellationToken);
        }

        public Task<TResponse> Execute<TRequest, TResponse>(TRequest requestData, CancellationToken cancellationToken)
            where TRequest : class, IRequest<TResponse>
        {
            var handler = _internals.GetService<IHandle<TRequest, TResponse>>() ?? throw new InvalidOperationException($"Request handler for {typeof(TRequest).Name} was not registered");
            return handler.Handle(requestData, cancellationToken);
        }
    }
}
