using Microsoft.Extensions.Configuration;
using System;
using TychoV2.Modules;

namespace TychoV2.Apps
{
    /// <summary>
    /// TODO
    /// </summary>
    public interface IAppStructure
    {
        /// <summary>
        /// TODO
        /// </summary>
        IAppStructure AddModule<TModule>(
            Action<IContractFulfillment>? contractFulfillment = null,
            Action<IConfigurationBuilder>? configurationDefinition = null)
            where TModule : TychoModule, new();
    }
}