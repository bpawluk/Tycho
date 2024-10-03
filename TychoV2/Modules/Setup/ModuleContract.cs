using Microsoft.Extensions.DependencyInjection;
using System;
using TychoV2.Requests;
using TychoV2.Requests.Broker;
using TychoV2.Requests.Registrating;
using TychoV2.Structure;

namespace TychoV2.Modules.Setup
{
    internal class ModuleContract : IModuleContract
    {
        private readonly Internals _internals;
        private readonly Registrator _registrator;

        private IRequestBroker? _contractFulfillingBroker;

        public ModuleContract(Internals internals)
        {
            _internals = internals;
            _registrator = new Registrator(_internals);
        }

        public void WithContractFulfillment(IRequestBroker contractFulfillingBroker)
        {
            _contractFulfillingBroker = contractFulfillingBroker;
            _internals.GetServiceCollection().AddSingleton(_contractFulfillingBroker!);
        }

        public IModuleContract Forwards<TRequest, TModule>()
            where TRequest : class, IRequest
            where TModule : TychoModule
        {
            _registrator.ForwardUpStreamRequest<TRequest, TModule>();
            return this;
        }

        public IModuleContract Forwards<TRequest, TResponse, TModule>()
            where TRequest : class, IRequest<TResponse>
            where TModule : TychoModule
        {
            _registrator.ForwardUpStreamRequest<TRequest, TResponse, TModule>();
            return this;
        }

        public IModuleContract Handles<TRequest, THandler>()
            where TRequest : class, IRequest
            where THandler : class, IHandle<TRequest>
        {
            _registrator.HandleUpStreamRequest<TRequest, THandler>();
            return this;
        }

        public IModuleContract Handles<TRequest, TResponse, THandler>()
            where TRequest : class, IRequest<TResponse>
            where THandler : class, IHandle<TRequest, TResponse>
        {
            _registrator.HandleUpStreamRequest<TRequest, TResponse, THandler>();
            return this;
        }

        public IModuleContract Requires<TRequest>()
            where TRequest : class, IRequest
        {
            if (!_contractFulfillingBroker!.CanExecute<TRequest>())
            {
                throw new InvalidOperationException($"Parent module does not handle the required {typeof(TRequest).Name} request");
            }
            return this;
        }

        public IModuleContract Requires<TRequest, TResponse>()
            where TRequest : class, IRequest<TResponse>
        {
            if (!_contractFulfillingBroker!.CanExecute<TRequest, TResponse>())
            {
                throw new InvalidOperationException($"Parent module does not handle the required {typeof(TRequest).Name} request");
            }
            return this;
        }
    }
}
