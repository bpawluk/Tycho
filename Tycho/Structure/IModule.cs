using System;
using Tycho.Events.Routing;
using Tycho.Modules;
using Tycho.Requests;
using Tycho.Structure.Data;

namespace Tycho.Structure
{
    /// <summary>
    /// An interface for a Tycho module
    /// </summary>
    public interface IModule : IRequestExecutor, IAsyncDisposable
    {
        internal Internals Internals { get; }

        internal IEventRouter EventRouter { get; }
    }

    /// <summary>
    /// An interface for a Tycho module defined by <typeparamref name="TModuleDefinition"/>
    /// </summary>
    /// <typeparam name="TModuleDefinition">The definition of the module</typeparam>
    public interface IModule<TModuleDefinition> : IModule
        where TModuleDefinition : TychoModule
    {
    }
}