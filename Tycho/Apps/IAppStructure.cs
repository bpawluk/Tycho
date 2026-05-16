using System;
using Tycho.Modules;
using Tycho.Utils;

namespace Tycho.Apps
{
    /// <summary>
    /// An interface for declaring the modules used by a Tycho application.
    /// </summary>
    [ReferencedBySourceGenerator]
    public interface IAppStructure
    {
        /// <summary>
        /// Declares that a module of type <typeparamref name="TModule"/> is used by the application.
        /// </summary>
        /// <typeparam name="TModule">The definition of the module to use.</typeparam>
        [ReferencedBySourceGenerator]
        IAppStructure Uses<TModule>()
            where TModule : TychoModule, new();

        /// <summary>
        /// Declares that a module of type <typeparamref name="TModule"/> is used by the application,
        /// together with how its contract is going to be fulfilled.
        /// </summary>
        /// <typeparam name="TModule">The definition of the module to use.</typeparam>
        /// <param name="contractFulfillment">The definition of how to fulfill the module contract.</param>
        /// <exception cref="ArgumentNullException"/>
        [ReferencedBySourceGenerator]
        IAppStructure Uses<TModule>(Action<IContractFulfillment> contractFulfillment)
            where TModule : TychoModule, new();

        /// <summary>
        /// Declares that a module of type <typeparamref name="TModule"/> is used by the application
        /// and passes the specified settings to it.
        /// </summary>
        /// <typeparam name="TModule">The definition of the module to use.</typeparam>
        /// <param name="settings">The settings for the module to use.</param>
        /// <exception cref="ArgumentNullException"/>
        [ReferencedBySourceGenerator]
        IAppStructure Uses<TModule>(IModuleSettings settings)
            where TModule : TychoModule, new();

        /// <summary>
        /// Declares that a module of type <typeparamref name="TModule"/> is used by the application,
        /// together with how its contract is going to be fulfilled, and passes the specified settings to it.
        /// </summary>
        /// <typeparam name="TModule">The definition of the module to use.</typeparam>
        /// <param name="contractFulfillment">The definition of how to fulfill the module contract.</param>
        /// <param name="settings">The settings for the module to use.</param>
        /// <exception cref="ArgumentNullException"/>
        [ReferencedBySourceGenerator]
        IAppStructure Uses<TModule>(
            Action<IContractFulfillment> contractFulfillment,
            IModuleSettings settings)
            where TModule : TychoModule, new();
    }
}
