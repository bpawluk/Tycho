using System;
using Tycho.Apps.Instance;
using Tycho.Utils;

namespace Tycho.Apps
{
    /// <summary>
    /// Base interface for generated application builders.
    /// </summary>
    [ReferencedBySourceGenerator]
    public interface IAppBuilderBase
    {
        /// <summary>
        /// Builds the application.
        /// </summary>
        IApp Build(IServiceProvider? parentServiceProvider);
    }
}
