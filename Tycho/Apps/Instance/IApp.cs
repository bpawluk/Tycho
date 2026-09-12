using System;
using Tycho.Requests.Broker;
using Tycho.Structure;
using Tycho.Utils;

namespace Tycho.Apps.Instance
{
    /// <summary>
    /// Represents a Tycho application instance.
    /// </summary>
    [ReferencedBySourceGenerator]
    public interface IApp : IRunnable, IDisposable
    {
        internal Internals Internals { get; }
        internal IRequestBroker RequestBroker { get; }
    }

    /// <summary>
    /// Represents a Tycho application instance defined by <typeparamref name="TAppDefinition"/>.
    /// </summary>
    /// <typeparam name="TAppDefinition">The application definition type.</typeparam>
    [ReferencedBySourceGenerator]
    public interface IApp<TAppDefinition> : IApp where TAppDefinition : TychoApp
    {
    }
}
