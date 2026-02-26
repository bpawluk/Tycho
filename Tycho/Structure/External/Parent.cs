using Tycho.Events.Routing;
using Tycho.Requests.Broker;

namespace Tycho.Structure.External
{
    internal class Parent : IParent
    {
        private readonly IEventRouter _parentEventRouter;
        private readonly IRequestBroker _contractFulfillingBroker;

        IEventRouter IParent.EventRouter => _parentEventRouter;
        IRequestBroker IParent.RequestBroker => _contractFulfillingBroker;

        public Parent(IEventRouter parentEventRouter, IRequestBroker contractFulfillingBroker)
        {
            _parentEventRouter = parentEventRouter;
            _contractFulfillingBroker = contractFulfillingBroker;
        }
    }
}