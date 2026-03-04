using Tycho.Events.Broker;
using Tycho.Requests.Broker;

namespace Tycho.Structure.Parent
{
    internal class ParentReference : IParentReference
    {
        private readonly IEventBroker _parentEventBroker;
        private readonly IRequestBroker _contractFulfillingBroker;

        IEventBroker IParentReference.EventBroker => _parentEventBroker;
        IRequestBroker IParentReference.RequestBroker => _contractFulfillingBroker;

        public ParentReference(IEventBroker parentEventBroker, IRequestBroker contractFulfillingBroker)
        {
            _parentEventBroker = parentEventBroker;
            _contractFulfillingBroker = contractFulfillingBroker;
        }
    }
}