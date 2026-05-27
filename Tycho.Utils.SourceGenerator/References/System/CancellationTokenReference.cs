using System.Threading;
using Tycho.Utils.SourceGenerator.Models.System;

namespace Tycho.Utils.SourceGenerator.References.System
{
    internal static class CancellationTokenReference
    {
        public static TypeModel TypeModel { get; } = new TypeModel(typeof(CancellationToken).Namespace, nameof(CancellationToken));
    }
}
