using System.Threading;
using System.Threading.Tasks;
using Tycho.Requests.Broker;
using Tycho.Structure;
using Tycho.Utils;

namespace Tycho.Apps.Instance
{
    [ReferencedByReflection]
    internal class App<TAppDefinition> : IApp<TAppDefinition> where TAppDefinition : TychoApp
    {
        private readonly Internals _internals;
        private readonly IRequestBroker _requestBroker;

        Internals IApp.Internals => _internals;
        IRequestBroker IApp.RequestBroker => _requestBroker;

        [ReferencedByReflection]
        public App(Internals internals)
        {
            _internals = internals;
            _requestBroker = new UpStreamBroker(_internals);
        }

        public Task StartAsync(CancellationToken cancellationToken) => _internals.StartAsync(cancellationToken);

        public Task StopAsync(CancellationToken cancellationToken) => _internals.StopAsync(cancellationToken);

        public void Dispose() => _internals.Dispose();
    }
}
