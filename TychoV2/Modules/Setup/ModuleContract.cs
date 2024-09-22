using TychoV2.Structure;

namespace TychoV2.Modules.Setup
{
    internal class ModuleContract : IModuleContract
    {
        private readonly Internals _internals;

        public ModuleContract(Internals internals)
        {
            _internals = internals;
        }

        public void Build()
        {
        }
    }
}
