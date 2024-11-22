using System;

namespace Tycho.Modules
{
    /// <summary>
    ///     TODO
    /// </summary>
    public interface IModuleStructure
    {
        public IModuleStructure Uses<TModule>()
            where TModule : TychoModule, new();

        public IModuleStructure Uses<TModule>(Action<IContractFulfillment> contractFulfillment)
            where TModule : TychoModule, new();

        public IModuleStructure Uses<TModule>(IModuleSettings settings)
            where TModule : TychoModule, new();

        public IModuleStructure Uses<TModule>(
            Action<IContractFulfillment> contractFulfillment,
            IModuleSettings settings)
            where TModule : TychoModule, new();
    }
}