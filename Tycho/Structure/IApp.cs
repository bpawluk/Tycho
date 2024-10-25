using System;
using Tycho.Apps;
using Tycho.Requests;

namespace Tycho.Structure
{
    /// <summary>
    ///     TODO
    /// </summary>
    public interface IApp : IRequestExecutor, IDisposable
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