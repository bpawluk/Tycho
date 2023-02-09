using Microsoft.Extensions.Configuration;
using System;
using Tycho.Contract;

namespace Tycho.Structure
{
    /// <summary>
    /// Lets you define which submodules will be part of your module and how it will use them
    /// </summary>
    public interface ISubstructureDefinition
    {
        /// <summary>
        /// Adds the specified module to the substructure of your module.
        /// Defines how it handles its outgoing messages and provides required configuration.
        /// </summary>
        /// <typeparam name="Module">A type that defines the submodule</typeparam>
        /// <param name="contractFulfillment">A method that defines how your module fulfills the submodule's contract</param>
        /// <param name="configurationDefinition">A method that defines the submodule's configuration</param>
        ISubstructureDefinition AddSubmodule<Module>(
            Action<IOutboxConsumer>? contractFulfillment = null,
            Action<IConfigurationBuilder>? configurationDefinition = null)
            where Module : TychoModule, new();
    }
}
