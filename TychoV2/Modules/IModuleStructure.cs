using Microsoft.Extensions.Configuration;
using System;

namespace TychoV2.Modules
{
    /// <summary>
    /// TODO
    /// </summary>
    public interface IModuleStructure
    {
        /// <summary>
        /// TODO
        /// </summary>
        IModuleStructure AddSubmodule<TModule>(
            Action<IContractFulfillment>? contractFulfillment = null,
            Action<IConfigurationBuilder>? configurationDefinition = null)
            where TModule : TychoModule, new();
    }
}