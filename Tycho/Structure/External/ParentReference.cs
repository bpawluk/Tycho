using Tycho.Events.Routing;
using Tycho.Requests.Broker;

namespace Tycho.Structure.External
{
    internal class ParentReference : IParentReference
    {
        private readonly IEventRouter _parentEventRouter;
        private readonly IRequestBroker _contractFulfillingBroker;

        IEventRouter IParentReference.EventRouter => _parentEventRouter;
        IRequestBroker IParentReference.RequestBroker => _contractFulfillingBroker;

        public ParentReference(IEventRouter parentEventRouter, IRequestBroker contractFulfillingBroker)
        {
            _parentEventRouter = parentEventRouter;
            _contractFulfillingBroker = contractFulfillingBroker;
        }
    }
}