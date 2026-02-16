using Tycho.Utils.SourceGenerator.Models.System;
using Tycho.Utils.SourceGenerator.Utils;

namespace Tycho.Utils.SourceGenerator.References.System
{
    internal static class VoidReference
    {
        public static TypeModel TypeModel { get; } = new TypeModel(typeof(void).Namespace, ImmutableEquatableArray<string>.Empty, typeof(void).Name);
    }
}
