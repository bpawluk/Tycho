using System;

namespace Tycho.Modules
{
    /// <summary>
    /// An interface for declaring the submodules used by a module
    /// </summary>
    public interface IModuleStructure
    {
        /// <summary>
        /// Declares that a module of type <typeparamref name="TModule"/> is used by the current module
        /// </summary>
        /// <typeparam name="TModule">The definition of the module to use</typeparam>
        public IModuleStructure Uses<TModule>()
            where TModule : TychoModule, new();

        /// <summary>
        /// Declares that a module of type <typeparamref name="TModule"/> is used by the current module
        /// together with how its contract is going to be fulfilled
        /// </summary>
        /// <typeparam name="TModule">The definition of the module to use</typeparam>
        /// <param name="contractFulfillment">The definition of how to fulfill the contract of the module to use</param>
        public IModuleStructure Uses<TModule>(Action<IContractFulfillment> contractFulfillment)
            where TModule : TychoModule, new();

        /// <summary>
        /// Declares that a module of type <typeparamref name="TModule"/> is used by the current module
        /// and passes the specified settings to it
        /// </summary>
        /// <typeparam name="TModule">The definition of the module to use</typeparam>
        /// <param name="settings">The settings for the module to use</param>
        public IModuleStructure Uses<TModule>(IModuleSettings settings)
            where TModule : TychoModule, new();

        /// <summary>
        /// Declares that a module of type <typeparamref name="TModule"/> is used by the current module
        /// together with how its contract is going to be fulfilled and passes the specified settings to it
        /// </summary>
        /// <typeparam name="TModule">The definition of the module to use</typeparam>
        /// <param name="contractFulfillment">The definition of how to fulfill the contract of the module to use</param>
        /// <param name="settings">The settings for the module to use</param>
        public IModuleStructure Uses<TModule>(
            Action<IContractFulfillment> contractFulfillment,
            IModuleSettings settings)
            where TModule : TychoModule, new();
    }
}