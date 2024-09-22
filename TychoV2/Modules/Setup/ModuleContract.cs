using TychoV2.Requests;
using TychoV2.Structure;

namespace TychoV2.Modules.Setup
{
    internal class ModuleContract : IModuleContract
    {
        private readonly Internals _internals;

        public ModuleContract(Internals internals)
        {
            _internals = internals;
        }

        public IModuleContract Forwards<TRequest, TModule>()
            where TRequest : class, IRequest
            where TModule : TychoModule
        {
            throw new System.NotImplementedException();
        }

        public IModuleContract Forwards<TRequest, TResponse, TModule>()
            where TRequest : class, IRequest<TResponse>
            where TModule : TychoModule
        {
            throw new System.NotImplementedException();
        }

        public IModuleContract Handles<TRequest, THandler>()
            where TRequest : class, IRequest
            where THandler : class, IHandle<TRequest>
        {
            throw new System.NotImplementedException();
        }

        public IModuleContract Handles<TRequest, TResponse, THandler>()
            where TRequest : class, IRequest<TResponse>
            where THandler : class, IHandle<TRequest, TResponse>
        {
            throw new System.NotImplementedException();
        }

        public IModuleContract Requires<TRequest>()
            where TRequest : class, IRequest
        {
            throw new System.NotImplementedException();
        }

        public IModuleContract Requires<TRequest, TResponse>()
            where TRequest : class, IRequest<TResponse>
        {
            throw new System.NotImplementedException();
        }

        public void Build()
        {
        }
    }
}
