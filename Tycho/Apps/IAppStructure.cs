using Microsoft.Extensions.Configuration;
using System;
using Tycho.Modules;

namespace Tycho.Apps
{
    /// <summary>
    /// TODO
    /// </summary>
    public interface IAppStructure
    {
        /// <summary>
        /// TODO
        /// </summary>
        IAppStructure Uses<TModule>(
            Action<IContractFulfillment>? contractFulfillment = null,
            Action<IConfigurationBuilder>? configurationDefinition = null)
            where TModule : TychoModule, new();
    }
}