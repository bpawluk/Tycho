using Tycho.Utils.SourceGenerator.Model.Generic;
using Tycho.Utils.SourceGenerator.Model.System;

namespace Tycho.Utils.SourceGenerator.References.System
{
    internal static class VoidReference
    {
        public static TypeModel TypeModel { get; } = new TypeModel(typeof(void).Namespace, ImmutableEquatableArray<string>.Empty, typeof(void).Name);
    }
}
