using System.Threading.Tasks;
using System.Threading;
using TychoV2.Requests.Broker;
using TychoV2.Structure;
using TychoV2.Requests;

namespace TychoV2.Apps.Instance
{
    internal class App<TAppDefinition> : IApp<TAppDefinition>
        where TAppDefinition : TychoApp
    {
        private readonly Internals _internals;

        Internals IApp.Internals => _internals;

        public App(Internals internals)
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
