using Tycho.Utils.SourceGenerator.Models.System;

namespace Tycho.Utils.SourceGenerator.References.System
{
    internal static class VoidReference
    {
        public static TypeModel TypeModel { get; } = new TypeModel(typeof(void).Namespace, typeof(void).Name);
    }
}
