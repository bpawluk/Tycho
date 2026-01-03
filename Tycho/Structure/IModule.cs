using System;
using Tycho.Events.Routing;
using Tycho.Modules;
using Tycho.Requests;
using Tycho.Structure.Internal;

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
    /// An interface for a Tycho module defined by <typeparamref name="TTychoDefinition"/>
    /// </summary>
    /// <typeparam name="TTychoDefinition">The definition of the module</typeparam>
    public interface IModule<TTychoDefinition> : IModule
        where TTychoDefinition : TychoModule
    {
    }
}