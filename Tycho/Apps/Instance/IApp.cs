using System;
using Tycho.Requests.Broker;
using Tycho.Structure.Internal;
using Tycho.Utils;

namespace Tycho.Apps.Instance
{
    /// <summary>
    /// Represents a running Tycho application instance.
    /// </summary>
    [ReferencedBySourceGenerator]
    public interface IApp : IAsyncDisposable
    {
        internal Internals Internals { get; }
        internal IRequestBroker RequestBroker { get; }
    }

    /// <summary>
    /// Represents a running Tycho application instance defined by <typeparamref name="TAppDefinition"/>.
    /// </summary>
    /// <typeparam name="TAppDefinition">The application definition type.</typeparam>
    [ReferencedBySourceGenerator]
    public interface IApp<TAppDefinition> : IApp where TAppDefinition : TychoApp
    {
    }
}
