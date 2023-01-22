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
        /// Adds the specified module to the substructure of your module
        /// </summary>
        /// <typeparam name="Module">A type that defines the submodule</typeparam>
        ISubstructureDefinition AddSubmodule<Module>()
            where Module : TychoModule, new();

        /// <summary>
        /// Adds the specified module to the substructure of your module 
        /// and defines how it handles its outgoing messages
        /// </summary>
        /// <typeparam name="Module">A type that defines the submodule</typeparam>
        /// <param name="contractFulfillment">A method that defines 
        /// how your module fulfills the submodule's contract</param>
        ISubstructureDefinition AddSubmodule<Module>(Action<IOutboxConsumer> contractFulfillment)
            where Module : TychoModule, new();
    }
}
