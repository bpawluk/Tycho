using TychoV2.Modules;
using TychoV2.Requests;
using TychoV2.Requests.Handling;
using TychoV2.Requests.Registrating;
using TychoV2.Structure;

namespace TychoV2.Apps.Setup
{
    internal class ContractFulfillment<TSourceModule> : IContractFulfillment
        where TSourceModule : TychoModule
    {
        private readonly Registrator _registrator;

        public ContractFulfillment(Internals internals)
        {
            _registrator = new Registrator(internals);
        }

        public IContractFulfillment Forward<TRequest, TTargetModule>()
            where TRequest : class, IRequest
            where TTargetModule : TychoModule
        {
            _registrator.ForwardDownStreamRequest<TSourceModule, TRequest, TTargetModule>();
            return this;
        }

        public IContractFulfillment Forward<TRequest, TResponse, TTargetModule>()
            where TRequest : class, IRequest<TResponse>
            where TTargetModule : TychoModule
        {
            _registrator.ForwardDownStreamRequest<TSourceModule, TRequest, TResponse, TTargetModule>();
            return this;
        }

        public IContractFulfillment Handle<TRequest, THandler>()
            where TRequest : class, IRequest
            where THandler : class, IRequestHandler<TRequest>
        {
            _registrator.HandleDownStreamRequest<TSourceModule, TRequest, THandler>();
            return this;
        }

        public IContractFulfillment Handle<TRequest, TResponse, THandler>()
            where TRequest : class, IRequest<TResponse>
            where THandler : class, IRequestHandler<TRequest, TResponse>
        {
            _registrator.HandleDownStreamRequest<TSourceModule, TRequest, TResponse, THandler>();
            return this;
        }

        public IContractFulfillment Ignore<TRequest>()
            where TRequest : class, IRequest
        {
            _registrator.HandleDownStreamRequest<TSourceModule, TRequest, RequestIgnorer<TRequest>>();
            return this;
        }

        public IContractFulfillment Ignore<TRequest, TResponse>()
            where TRequest : class, IRequest<TResponse>
        {
            _registrator.HandleDownStreamRequest<TSourceModule, TRequest, TResponse, RequestIgnorer<TRequest, TResponse>>();
            return this;
        }
    }
}
