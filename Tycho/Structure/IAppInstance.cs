using System;
using Tycho.Apps;
using Tycho.Requests;
using Tycho.Structure.Internal;
using Tycho.Utils;

namespace Tycho.Structure
{
    /// <summary>
    /// Represents a running Tycho application instance.
    /// </summary>
    [ReferencedBySourceGenerator]
    public interface IAppInstance : IRequestExecutor, IAsyncDisposable
    {
        internal Internals Internals { get; }
    }

    /// <summary>
    /// Represents a running Tycho application instance defined by <typeparamref name="TAppDefinition"/>.
    /// </summary>
    /// <typeparam name="TAppDefinition">The application definition type.</typeparam>
    [ReferencedBySourceGenerator]
    public interface IAppInstance<TAppDefinition> : IAppInstance
        where TAppDefinition : TychoApp
    {
    }
}
