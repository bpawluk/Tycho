using Tycho.Requests;
using Tycho.Requests.Registrating;
using Tycho.Structure;

namespace Tycho.Apps.Setup
{
    internal class AppContract : IAppContract
    {
        private readonly Registrator _registrator;

        public AppContract(Internals internals)
        {
            _registrator = new Registrator(internals);
        }

        public IAppRequestBinding<TRequest> Expects<TRequest>()
            where TRequest : class, IRequest
        {
            return new AppRequestBinding<TRequest>(this, _registrator);
        }

        public IAppRequestBinding<TRequest, TResponse> Expects<TRequest, TResponse>()
            where TRequest : class, IRequest<TResponse>
        {
            return new AppRequestBinding<TRequest, TResponse>(this, _registrator);
        }

    }
}
