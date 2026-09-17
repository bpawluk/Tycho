using System;
using Tycho.Utils.SourceGenerator.Models.System;

namespace Tycho.Utils.SourceGenerator.References.System
{
    internal static class ArgumentNullExceptionReference
    {
        public static TypeReferenceModel TypeModel { get; } = new TypeReferenceModel(typeof(ArgumentNullException).Namespace, nameof(ArgumentNullException));
    }
}
