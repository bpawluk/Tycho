using TychoV2.Modules;
using TychoV2.Requests;
using TychoV2.Requests.Registrator;
using TychoV2.Structure;

namespace TychoV2.Apps.Setup
{
    internal class AppContract : IAppContract
    {
        private readonly Registrator _registrator;

        public AppContract(Internals internals)
        {
            _registrator = new Registrator(internals);
        }

        public IAppContract Forwards<TRequest, TModule>()
            where TRequest : class, IRequest
            where TModule : TychoModule
        {
            _registrator.ForwardUpStreamRequest<TRequest, TModule>();
            return this;
        }

        public IAppContract Forwards<TRequest, TResponse, TModule>()
            where TRequest : class, IRequest<TResponse>
            where TModule : TychoModule
        {
            _registrator.ForwardUpStreamRequest<TRequest, TResponse, TModule>();
            return this;
        }

        public IAppContract Handles<TRequest, THandler>()
            where TRequest : class, IRequest
            where THandler : class, IHandle<TRequest>
        {
            _registrator.HandleUpStreamRequest<TRequest, THandler>();
            return this;
        }

        public IAppContract Handles<TRequest, TResponse, THandler>()
            where TRequest : class, IRequest<TResponse>
            where THandler : class, IHandle<TRequest, TResponse>
        {
            _registrator.HandleUpStreamRequest<TRequest, TResponse, THandler>();
            return this;
        }
    }
}
