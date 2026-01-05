using System;
using Tycho.Apps;
using Tycho.Requests;
using Tycho.Structure.Internal;

namespace Tycho.Structure
{
    /// <summary>
    /// Represents a running Tycho application instance.
    /// </summary>
    public interface IApp : IRequestExecutor, IAsyncDisposable
    {
        internal Internals Internals { get; }
    }

    /// <summary>
    /// Represents a running Tycho application instance defined by <typeparamref name="TAppDefinition"/>.
    /// </summary>
    /// <typeparam name="TAppDefinition">The application definition type.</typeparam>
    public interface IApp<TAppDefinition> : IApp
        where TAppDefinition : TychoApp
    {
    }
}
