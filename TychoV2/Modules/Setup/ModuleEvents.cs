using TychoV2.Events;
using TychoV2.Modules.Routing;
using TychoV2.Structure;

namespace TychoV2.Modules.Setup
{
    internal class ModuleEvents : IModuleEvents
    {
        private readonly Internals _internals;

        public ModuleEvents(Internals internals)
        {
            _internals = internals;
        }

        public IModuleContract Handles<TEvent, THandler>()
            where TEvent : class, IEvent
            where THandler : class, IHandle<TEvent>
        {
            throw new System.NotImplementedException();
        }

        public IEventRouting Routes<TEvent>()
            where TEvent : class, IEvent
        {
            throw new System.NotImplementedException();
        }

        public void Build()
        {
        }
    }
}
