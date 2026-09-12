using System;
using Tycho.Utils.SourceGenerator.Models.System;

namespace Tycho.Utils.SourceGenerator.References.System
{
    internal static class InvalidOperationExceptionReference
    {
        public static TypeReferenceModel TypeModel { get; } = new TypeReferenceModel(typeof(InvalidOperationException).Namespace, nameof(InvalidOperationException));
    }
}
