using System.Threading;
using System.Threading.Tasks;
using TychoV2.Requests;
using TychoV2.Requests.Broker;
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
            var broker = new UpStreamBroker(_internals);
            return broker.Execute(requestData, cancellationToken);
        }

        public Task<TResponse> Execute<TRequest, TResponse>(TRequest requestData, CancellationToken cancellationToken)
            where TRequest : class, IRequest<TResponse>
        {
            var broker = new UpStreamBroker(_internals);
            return broker.Execute<TRequest, TResponse>(requestData, cancellationToken);
        }
    }
}
