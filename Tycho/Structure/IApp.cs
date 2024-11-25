using System;
using Tycho.Apps;
using Tycho.Requests;
using Tycho.Structure.Data;

namespace Tycho.Structure
{
    /// <summary>
    /// An interface for a Tycho application
    /// </summary>
    public interface IApp : IRequestExecutor, IAsyncDisposable
    {
        internal Internals Internals { get; }
    }

    /// <summary>
    /// An interface for a Tycho application defined by <typeparamref name="TAppDefinition"/>
    /// </summary>
    /// <typeparam name="TAppDefinition">The definition of the application</typeparam>
    public interface IApp<TAppDefinition> : IApp
        where TAppDefinition : TychoApp
    {
    }
}