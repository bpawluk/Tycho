using System;
using Tycho.Utils.SourceGenerator.Models.System;

namespace Tycho.Utils.SourceGenerator.References.System
{
    internal static class BooleanReference
    {
        public static TypeReferenceModel TypeModel { get; } = new TypeReferenceModel(typeof(bool).Namespace, nameof(Boolean));
    }
}
