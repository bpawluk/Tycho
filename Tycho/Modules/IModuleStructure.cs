using System;
using Tycho.Utils;

namespace Tycho.Modules
{
    /// <summary>
    /// An interface for declaring the submodules used by a Module.
    /// </summary>
    [ReferencedBySourceGenerator]
    public interface IModuleStructure
    {
        /// <summary>
        /// Declares that a Module of type <typeparamref name="TModule"/> is used by the current Module.
        /// </summary>
        /// <typeparam name="TModule">The definition of the Module to use.</typeparam>
        [ReferencedBySourceGenerator]
        IModuleStructure Uses<TModule>()
            where TModule : TychoModule, new();

        /// <summary>
        /// Declares that a Module of type <typeparamref name="TModule"/> is used by the current Module,
        /// together with how its contract is going to be fulfilled.
        /// </summary>
        /// <typeparam name="TModule">The definition of the Module to use.</typeparam>
        /// <param name="contractFulfillment">The definition of how to fulfill the Module contract.</param>
        /// <exception cref="ArgumentNullException"/>
        [ReferencedBySourceGenerator]
        IModuleStructure Uses<TModule>(Action<IContractFulfillment> contractFulfillment)
            where TModule : TychoModule, new();

        /// <summary>
        /// Declares that a Module of type <typeparamref name="TModule"/> is used by the current Module
        /// and passes the specified settings to it.
        /// </summary>
        /// <typeparam name="TModule">The definition of the Module to use.</typeparam>
        /// <param name="settings">The settings for the Module to use.</param>
        /// <exception cref="ArgumentNullException"/>
        [ReferencedBySourceGenerator]
        IModuleStructure Uses<TModule>(IModuleSettings settings)
            where TModule : TychoModule, new();

        /// <summary>
        /// Declares that a Module of type <typeparamref name="TModule"/> is used by the current Module,
        /// together with how its contract is going to be fulfilled, and passes the specified settings to it.
        /// </summary>
        /// <typeparam name="TModule">The definition of the Module to use.</typeparam>
        /// <param name="contractFulfillment">The definition of how to fulfill the Module contract.</param>
        /// <param name="settings">The settings for the Module to use.</param>
        /// <exception cref="ArgumentNullException"/>
        [ReferencedBySourceGenerator]
        IModuleStructure Uses<TModule>(
            Action<IContractFulfillment> contractFulfillment,
            IModuleSettings settings)
            where TModule : TychoModule, new();
    }
}
