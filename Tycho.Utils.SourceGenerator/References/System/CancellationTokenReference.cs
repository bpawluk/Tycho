using System.Threading;
using Tycho.Utils.SourceGenerator.Models.System;

namespace Tycho.Utils.SourceGenerator.References.System
{
    internal static class CancellationTokenReference
    {
        public static TypeReferenceModel TypeModel { get; } = new TypeReferenceModel(typeof(CancellationToken).Namespace, nameof(CancellationToken));
    }
}
