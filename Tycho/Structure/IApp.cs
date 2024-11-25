using System;
using Tycho.Apps;
using Tycho.Requests;
using Tycho.Structure.Data;

namespace Tycho.Structure
{
    /// <summary>
    ///     TODO
    /// </summary>
    public interface IApp : IRequestExecutor, IAsyncDisposable
    {
        internal Internals Internals { get; }
    }

    /// <summary>
    ///     TODO
    /// </summary>
    public interface IApp<TAppDefinition> : IApp
        where TAppDefinition : TychoApp
    {
    }
}