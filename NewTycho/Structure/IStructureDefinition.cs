using Microsoft.Extensions.Configuration;
using NewTycho.Modules;
using System;

namespace NewTycho.Structure
{
    /// <summary>
    /// Lets you define which submodules will be part of your module and how it will use them
    /// </summary>
    public interface IStructureDefinition
    {
        /// <summary>
        /// Adds the specified module to the substructure of your module.
        /// Defines how it handles its outgoing messages.
        /// </summary>
        /// <typeparam name="TModule">A type that defines the submodule</typeparam>
        /// <param name="contractFulfillment">A method that defines how your module fulfills the submodule's contract</param>
        IStructureDefinition AddModule<TModule>(Action<IOutboxConsumer> contractFulfillment)
            where TModule : TychoModule, new();

        /// <summary>
        /// Adds the specified module to the substructure of your module.
        /// Provides required configuration.
        /// </summary>
        /// <typeparam name="TModule">A type that defines the submodule</typeparam>
        /// <param name="configurationDefinition">A method that defines the submodule's configuration</param>
        IStructureDefinition AddModule<TModule>(Action<IConfigurationBuilder> configurationDefinition)
            where TModule : TychoModule, new();

        /// <summary>
        /// Adds the specified module to the substructure of your module.
        /// Defines how it handles its outgoing messages and provides required configuration.
        /// </summary>
        /// <typeparam name="TModule">A type that defines the submodule</typeparam>
        /// <param name="contractFulfillment">A method that defines how your module fulfills the submodule's contract</param>
        /// <param name="configurationDefinition">A method that defines the submodule's configuration</param>
        IStructureDefinition AddModule<TModule>(
            Action<IOutboxConsumer> contractFulfillment,
            Action<IConfigurationBuilder> configurationDefinition)
            where TModule : TychoModule, new();
    }
}
