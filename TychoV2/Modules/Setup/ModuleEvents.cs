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

        public void Build()
        {
        }
    }
}
