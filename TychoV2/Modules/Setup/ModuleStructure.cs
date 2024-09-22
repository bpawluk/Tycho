using TychoV2.Structure;

namespace TychoV2.Modules.Setup
{
    internal class ModuleStructure : IModuleStructure
    {
        private readonly Internals _internals;

        public ModuleStructure(Internals internals)
        {
            _internals = internals;
        }

        public void Build()
        {
        }
    }
}
