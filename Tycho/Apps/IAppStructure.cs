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
        /// <param name="instanceSuffix">An optional suffix that distinguishes instances of the same module across the application.</param>
        [ReferencedBySourceGenerator]
        IAppStructure Uses<TModule>(string? instanceSuffix = null)
            where TModule : TychoModule, new();

        /// <summary>
        /// Declares that a module of type <typeparamref name="TModule"/> is used by the application, together with how its contract is going to be fulfilled.
        /// </summary>
        /// <typeparam name="TModule">The definition of the module to use.</typeparam>
        /// <param name="contractFulfillment">The definition of how to fulfill the module contract.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <param name="instanceSuffix">An optional suffix that distinguishes instances of the same module across the application.</param>
        [ReferencedBySourceGenerator]
        IAppStructure Uses<TModule>(Action<IContractFulfillment> contractFulfillment, string? instanceSuffix = null)
            where TModule : TychoModule, new();

        /// <summary>
        /// Declares that a module of type <typeparamref name="TModule"/> is used by the application and passes the specified settings to it.
        /// </summary>
        /// <typeparam name="TModule">The definition of the module to use.</typeparam>
        /// <param name="settings">The settings for the module to use.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <param name="instanceSuffix">An optional suffix that distinguishes instances of the same module across the application.</param>
        [ReferencedBySourceGenerator]
        IAppStructure Uses<TModule>(IModuleSettings settings, string? instanceSuffix = null)
            where TModule : TychoModule, new();

        /// <summary>
        /// Declares that a module of type <typeparamref name="TModule"/> is used by the application, together with how its contract is going to be fulfilled, and passes the specified settings to it.
        /// </summary>
        /// <typeparam name="TModule">The definition of the module to use.</typeparam>
        /// <param name="contractFulfillment">The definition of how to fulfill the module contract.</param>
        /// <param name="settings">The settings for the module to use.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <param name="instanceSuffix">An optional suffix that distinguishes instances of the same module across the application.</param>
        [ReferencedBySourceGenerator]
        IAppStructure Uses<TModule>(
            Action<IContractFulfillment> contractFulfillment,
            IModuleSettings settings,
            string? instanceSuffix = null)
            where TModule : TychoModule, new();
    }
}
