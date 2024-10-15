﻿using System;
using Microsoft.Extensions.Configuration;

namespace Tycho.Modules
{
    /// <summary>
    ///     TODO
    /// </summary>
    public interface IModuleStructure
    {
        /// <summary>
        ///     TODO
        /// </summary>
        IModuleStructure Uses<TModule>(
            Action<IContractFulfillment>? contractFulfillment = null,
            Action<IConfigurationBuilder>? configurationDefinition = null)
            where TModule : TychoModule, new();
    }
}