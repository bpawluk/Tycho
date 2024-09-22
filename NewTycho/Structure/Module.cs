using Microsoft.Extensions.DependencyInjection;
using NewTycho.Events.Publishing;
using NewTycho.Requests.Execution;
using System;

namespace NewTycho.Structure
{
    internal class Module
    {
        private IServiceCollection _serviceCollection = null!;

        private EventBroker _eventBroker = null!;
        private RequestBroker _externalRequestBroker = null!;
        private RequestBroker _internalRequestBroker = null!;

        private Module[] _submodules = null!;

        public IServiceProvider Services { get; private set; }

        public void SetServices(IServiceCollection services)
        {
            _serviceCollection = services;
        }

        public void SetEventBroker(EventBroker eventBroker)
        {
            _eventBroker = eventBroker;
        }

        public void SetExternalRequestBroker(RequestBroker externalRequestBroker)
        {
            _externalRequestBroker = externalRequestBroker;
        }

        public void SetInternalRequestBroker(RequestBroker publicRequestBroker)
        {
            _internalRequestBroker = publicRequestBroker;
        }

        public void SetSubmodules(Module[] submodules)
        {
        }

        public void Start()
        {
            Services = _serviceCollection.BuildServiceProvider();
        }
    }   
}
