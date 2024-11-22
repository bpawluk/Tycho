using System;
using Tycho.Modules;

namespace Tycho.Apps
{
    /// <summary>
    ///     TODO
    /// </summary>
    public interface IAppStructure
    {
        public IAppStructure Uses<TModule>()
            where TModule : TychoModule, new();

        public IAppStructure Uses<TModule>(Action<IContractFulfillment> contractFulfillment)
            where TModule : TychoModule, new();

        public IAppStructure Uses<TModule>(IModuleSettings settings)
            where TModule : TychoModule, new();

        public IAppStructure Uses<TModule>(
            Action<IContractFulfillment> contractFulfillment,
            IModuleSettings settings)
            where TModule : TychoModule, new();
    }
}