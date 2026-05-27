using Tycho.Utils.SourceGenerator.Models.System;

namespace Tycho.Utils.SourceGenerator.References.System
{
    internal static class VoidReference
    {
        public static TypeReferenceModel TypeModel { get; } = new TypeReferenceModel(typeof(void).Namespace, typeof(void).Name);
    }
}
